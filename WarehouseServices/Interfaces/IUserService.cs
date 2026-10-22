using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;

namespace WarehouseServices.Interfaces;

public interface IUserService
{
    Task<UserDto> GetUserByIdAsync(int userId, CancellationToken ct);
    Task<UserDto> GetUserByUsernameAsync(string username, CancellationToken ct);
    Task<UserDto> GetUserByEmailAsync(string email, CancellationToken ct);
    Task<List<UserDto>> GetAllUsersAsync(CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetUsersCountAsync(CancellationToken ct);
    Task<int> AddNewUserAsync(CreateUserDto userDto, CancellationToken ct);
    Task UpdateUserAsync(int userId, CreateUserDto userDto, CancellationToken ct);
    Task DeleteUserAsync(int userId, CancellationToken ct);
    Task<bool> IsUserExistsByIdAsync(int userId, CancellationToken ct);
    Task<bool> IsUserExistsByUsernameAsync(string username, CancellationToken ct);
    Task<bool> IsUserExistsByEmailAsync(string email, CancellationToken ct);
}
