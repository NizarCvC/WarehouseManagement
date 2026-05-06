using WarehouseDataAccess.Interfaces;
using WarehouseDataAccess.Repositories;
using WarehouseServices.Interfaces;
using WarehouseServices.Services;

namespace WarehouseAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddWarehouseServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserServices>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}   
