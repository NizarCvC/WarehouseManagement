using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;

namespace WarehouseDataAccess.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(int userId, CancellationToken ct);
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct);
    Task<List<User>> GetAllUsersAsync(CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetUsersCountAsync(CancellationToken ct);
    Task<int> AddNewUserAsync(CreateUserDto user, CancellationToken ct);
    Task<bool> UpdateUserAsync(int userId, CreateUserDto user, CancellationToken ct);
    Task<bool> DeactivateUserAsync(int userId, CancellationToken ct);
    Task<bool> IsUserIdExistsAsync(int userId, CancellationToken ct);
    Task<bool> IsUsernameExistsAsync(string username, CancellationToken ct);
    Task<bool> UpdateRefreshTokenAsync(int userId, string? refreshToken, DateTime? expiryTime, CancellationToken ct);
}
