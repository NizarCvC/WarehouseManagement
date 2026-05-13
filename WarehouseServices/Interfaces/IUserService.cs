using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;

namespace WarehouseServices.Interfaces;

public interface IUserService
{
    Task<UserDto> GetUserByIdAsync(int userId, CancellationToken ct);
    Task<UserDto> GetUserByUsernameAsync(string username, CancellationToken ct);
    Task<List<UserDto>> GetAllUsersAsync(CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetUsersCountAsync(CancellationToken ct);
    Task<int> AddNewUserAsync(CreateUserDto userDto, CancellationToken ct);
    Task UpdateUserAsync(int userId, CreateUserDto userDto, CancellationToken ct);
    Task DeactivateUserAsync(int userId, CancellationToken ct);
    Task<bool> IsUserIdExistsAsync(int userId, CancellationToken ct);
    Task<bool> IsUsernameExistsAsync(string username, CancellationToken ct);
}
