using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;
using WarehouseDataAccess.Interfaces;
using WarehouseServices.Exceptions;
using WarehouseServices.Interfaces;
using WarehouseServices.Security;
namespace WarehouseServices.Services;

public class UserServices(IUserRepository userRepository) : IUserService
{
    public async Task<UserDto?> GetUserByIdAsync(int userId, CancellationToken ct)
    {
        if (!await userRepository.IsUserIdExists(userId, ct))
            throw new NotFoundException($"The user ID: {userId} not exists.");

        User? userInfo = await userRepository.GetUserByIdAsync(userId, ct);

        return (userInfo is not null) ? UserDto.FromModel(userInfo) : throw new InternalServerErrorException();
    }

    public async Task<UserDto?> GetUserByUsernameAsync(string username, CancellationToken ct)
    {
        if (!await userRepository.IsUsernameExists(username, ct))
            throw new NotFoundException($"The user by username '{username}' not exists.");

        var userInfo = await userRepository.GetByUsernameAsync(username, ct);

        return (userInfo is not null) ? UserDto.FromModel(userInfo) : throw new InternalServerErrorException();
    }

    public async Task<List<UserDto>> GetAllUsersAsync(CancellationToken ct, int page = 1, int pageSize = 10)
    {
        List<User> users = await userRepository.GetAllUsersAsync(ct, page, pageSize);
        return UserDto.FromModels(users);
    }

    public async Task<int> AddNewUserAsync(CreateUserDto userDto, CancellationToken ct)
    {
        if (await userRepository.IsUsernameExists(userDto.Username, ct))
            throw new ConflictException($"The user with username '{userDto.Username}' is already used.");

        userDto.Password = Hashing.HashPasswordOnly(userDto.Password);
        int newId = await userRepository.AddNewUserAsync(userDto, ct);

        if (newId == -1)
            throw new InternalServerErrorException();

        return newId;
    }

    public async Task<bool> UpdateUserAsync(int userId, CreateUserDto userDto, CancellationToken ct)
    {
        var userInfo = await userRepository.GetUserByIdAsync(userId, ct);

        if (userInfo is null)
            throw new NotFoundException($"The user ID: {userId} not exists.");

        bool isUsernameUsed = await userRepository.IsUsernameExists(userDto.Username, ct);

        if (userInfo.Username != userDto.Username && isUsernameUsed)
            throw new ConflictException($"The username: {userDto.Username} is already used.");

        string userDtoPassword = Hashing.HashPasswordOnly(userDto.Password);

        userDto.Password = (userDtoPassword != userInfo.PasswordHash) ?
            Hashing.HashPasswordOnly(userDto.Password)
            : userDto.Password = userInfo.PasswordHash;

        bool isSuccesses = await userRepository.UpdateUserAsync(userId, userDto, ct);

        return isSuccesses ? true : throw new InternalServerErrorException();
    }

    public async Task<bool> DeleteUserAsync(int userId, CancellationToken ct)
    {
        if (!await userRepository.IsUserIdExists(userId, ct))
            throw new NotFoundException($"The user with ID: {userId} not exists.");

        bool isSuccesses = await userRepository.DeleteUserAsync(userId, ct);

        return isSuccesses ? true : throw new InternalServerErrorException();
    }

    public async Task<bool> IsUserIdExists(int userId, CancellationToken ct)
    {
        return await userRepository.IsUserIdExists(userId, ct);
    }

    public async Task<bool> IsUsernameExists(string username, CancellationToken ct)
    {
        return await userRepository.IsUsernameExists(username, ct);
    }

    public async Task<int> GetUsersCountAsync(CancellationToken ct)
    {
        return await userRepository.GetUsersCountAsync(ct);
    }
}
