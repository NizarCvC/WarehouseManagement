using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;

namespace WarehouseServices.Interfaces;

public interface IUserService
{
    Task<UserDto> GetUserByIdAsync(int userId, CancellationToken ct);
    Task<List<UserDto>> GetAllUsersAsync(CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetUsersCountAsync(CancellationToken ct);
    Task<int> AddNewUserAsync(CreateUserDto user, CancellationToken ct);
    Task<bool> UpdateUserAsync(CreateUserDto user, CancellationToken ct);
    Task<bool> DeleteUserAsync(int userId, CancellationToken ct);
}
