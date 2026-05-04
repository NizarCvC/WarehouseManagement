using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;
using WarehouseDataAccess.Interfaces;
using WarehouseServices.Interfaces;
using WarehouseServices.securty;
namespace WarehouseServices.Services;

public class UserServices : IUserService
{
    public IUserRepository _userRepository;

    public UserServices(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<int> AddNewUserAsync(CreateUserDto user, CancellationToken ct)
    {
        if (await _userRepository.IsUsernameExists(user.Username, ct))
            return -1;

        user.Password = Hashing.HashPasswordOnly(user.Password);

        int newId = await _userRepository.AddNewUserAsync(user, ct);
        return newId;
    }

    public async Task<bool> UpdateUserAsync(int userId, CreateUserDto user, CancellationToken ct)
    {
        if (!await _userRepository.IsUserIdExists(userId, ct))
            return false;

        bool isUpdated = await _userRepository.UpdateUserAsync(userId, user, ct);

        return isUpdated;
    }

    public async Task<bool> DeleteUserAsync(int userId, CancellationToken ct)
    {
        bool isDeleted = await _userRepository.DeleteUserAsync(userId, ct);
        return isDeleted;
    }

    public async Task<List<UserDto>> GetAllUsersAsync(CancellationToken ct, int page = 1, int pageSize = 10)
    {
        List<User> users = await _userRepository.GetAllUsersAsync(ct, page, pageSize);

        return UserDto.FromModels(users);
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId, CancellationToken ct)
    {
        User? userInfo = await _userRepository.GetUserByIdAsync(userId, ct);

        return (userInfo is not null) ? UserDto.FromModel(userInfo) : null;
    }

    public async Task<int> GetUsersCountAsync(CancellationToken ct)
    {
        int countUsers = await _userRepository.GetUsersCountAsync(ct);

        return countUsers;
    }
}
