using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;
using WarehouseDataAccess.Interfaces;
using WarehouseServices.Interfaces;
using WarehouseServices.securty;
namespace WarehouseServices.Services;

public class UserServices(IUserRepository userRepository) : IUserService
{
    public async Task<int> AddNewUserAsync(CreateUserDto user, CancellationToken ct)
    {
        user.Password = Hashing.HashPasswordOnly(user.Password);

        int newId = await userRepository.AddNewUserAsync(user, ct);
        return newId;
    }

    public async Task<bool> UpdateUserAsync(int userId, CreateUserDto user, CancellationToken ct)
    {
        return await userRepository.UpdateUserAsync(userId, user, ct);
    }

    public async Task<bool> DeleteUserAsync(int userId, CancellationToken ct)
    {
        return await userRepository.DeleteUserAsync(userId, ct);
    }

    public async Task<List<UserDto>> GetAllUsersAsync(CancellationToken ct, int page = 1, int pageSize = 10)
    {
        List<User> users = await userRepository.GetAllUsersAsync(ct, page, pageSize);

        return UserDto.FromModels(users);
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId, CancellationToken ct)
    {
        User? userInfo = await userRepository.GetUserByIdAsync(userId, ct);

        return (userInfo is not null) ? UserDto.FromModel(userInfo) : null;
    }

    public async Task<int> GetUsersCountAsync(CancellationToken ct)
    {
        int countUsers = await userRepository.GetUsersCountAsync(ct);

        return countUsers;
    }

    public async Task<bool> IsUserIdExists(int userId, CancellationToken ct)
    {
        return await userRepository.IsUserIdExists(userId, ct);
    }

    public async Task<bool> IsUsernameExists(string username, CancellationToken ct)
    {
        return await userRepository.IsUsernameExists(username, ct);
    }
}
