using System.Text;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using WarehouseAPI.Services;
using WarehouseCore.enums;
using WarehouseDataAccess.Interfaces;
using WarehouseDataAccess.Repositories;
using WarehouseServices.Interfaces;
using WarehouseServices.Services;

namespace WarehouseAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddWarehouseServices();
        services.AddApiDocumentation();
        services.AddJwtAuthentication(configuration);
        services.AddJwtAuthorization();
        services.AddProblemDetailsServices();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddRateLimiting();
        services.AddTracing(configuration);
        services.AddApiVersioningServices();
        services.AddMemoryCache(options => options.SizeLimit = 100);

        return services;
    }

    private static IServiceCollection AddWarehouseServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserServices>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }

    private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var jwtKey = configuration["Jwt:Key"];
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
            };
        });

        return services;
    }

    private static IServiceCollection AddJwtAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(enRole.SystemAdministrator.ToRoleName(), policy => policy.RequireRole(enRole.SystemAdministrator.ToRoleName()));
            options.AddPolicy(enRole.WarehouseManager.ToRoleName(), policy => policy.RequireRole(enRole.WarehouseManager.ToRoleName()));
            options.AddPolicy(enRole.SalesRepresentative.ToRoleName(), policy => policy.RequireRole(enRole.SalesRepresentative.ToRoleName()));
            options.AddPolicy(enRole.PurchasingOfficer.ToRoleName(), policy => policy.RequireRole(enRole.PurchasingOfficer.ToRoleName()));
            options.AddPolicy(enRole.Accountant.ToRoleName(), policy => policy.RequireRole(enRole.Accountant.ToRoleName()));
        });

        return services;
    }

    private static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        string[] versions = ["v1"];

        foreach (var version in versions)
        {
            services.AddOpenApi(version, options =>
            {
                options.AddDocumentTransformer<VersionInfoTransformer>();
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
                options.AddOperationTransformer<BearerSecurityOperationTransformer>();
            });
        }

        return services;
    }

    private static IServiceCollection AddProblemDetailsServices(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
            options.CustomizeProblemDetails = (context) =>
            {
                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
            }
        );

        return services;
    }

    private static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy("GeneralPolicy", context =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown_ip";
                return RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey: ipAddress,
                    factory: partition => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        SegmentsPerWindow = 4,
                        QueueLimit = 2,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                    });
            });

            options.AddPolicy("LoginPolicy", context =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown_ip";

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: ipAddress,
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(1)
                    });
            });

            options.AddPolicy("RefreshPolicy", context =>
            {
                var partitionKey = context.User.Identity?.IsAuthenticated == true
                    ? context.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? "unknown_user"
                    : context.Connection.RemoteIpAddress?.ToString() ?? "unknown_ip";

                return RateLimitPartition.GetTokenBucketLimiter(
                    partitionKey: partitionKey,
                    factory: partition => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 10,
                        ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                        TokensPerPeriod = 5,
                        AutoReplenishment = true
                    });
            });
        });

        return services;
    }

    private static IServiceCollection AddTracing(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetry()
        .ConfigureResource(res => res.AddService(configuration["warehouseapi"] ?? "warehouseapi"))
        .WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation().
            AddHttpClientInstrumentation();
            tracing.AddOtlpExporter();
        });

        return services;
    }

    private static IServiceCollection AddApiVersioningServices(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new HeaderApiVersionReader("api-version");
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = false;
        });

        return services;
    }
}
