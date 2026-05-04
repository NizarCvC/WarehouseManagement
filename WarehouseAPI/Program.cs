using WarehouseDataAccess.Interfaces;
using WarehouseDataAccess.Repositories;
using WarehouseServices.Interfaces;
using WarehouseServices.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserServices>();

var app = builder.Build();

app.MapControllers();

app.Run();
