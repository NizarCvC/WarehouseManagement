using WarehouseAPI.Filters;
using WarehouseDataAccess.Interfaces;
using WarehouseDataAccess.Repositories;
using WarehouseServices.Interfaces;
using WarehouseServices.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserServices>();
builder.Services.AddProblemDetails(options =>

    options.CustomizeProblemDetails = (context) =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.Add("requestid", context.HttpContext.TraceIdentifier);
    }
);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();
app.MapControllers();

app.Run();
