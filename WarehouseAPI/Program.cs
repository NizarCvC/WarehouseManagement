using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WarehouseAPI;
using WarehouseAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails(options =>
    options.CustomizeProblemDetails = (context) =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
    }
);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var jwtKey = builder.Configuration["Jwt:Key"];
    options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("System Administrator", policy => policy.RequireRole("System Administrator"));
    options.AddPolicy("Warehouse Manager", policy => policy.RequireRole("Warehouse Manager"));
    options.AddPolicy("Sales Representative", policy => policy.RequireRole("Sales Representative"));
    options.AddPolicy("Purchasing Officer", policy => policy.RequireRole("Purchasing Officer"));
    options.AddPolicy("Accountant", policy => policy.RequireRole("Accountant"));
});
builder.Services.AddWarehouseServices();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();