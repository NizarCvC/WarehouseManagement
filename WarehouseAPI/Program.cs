using WarehouseAPI;
using WarehouseAPI.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails(options =>
    options.CustomizeProblemDetails = (context) =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
    }
);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddWarehouseServices();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages(); 

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();