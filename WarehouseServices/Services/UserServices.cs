using Microsoft.Extensions.Logging;
using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;
using WarehouseDataAccess.Interfaces;
using WarehouseServices.Exceptions;
using WarehouseServices.Interfaces;
using WarehouseServices.Security;

namespace WarehouseServices.Services;

public class UserServices(IUserRepository userRepository, ILogger<UserServices> logger) : IUserService
{
    public async Task<UserDto> GetUserByIdAsync(int userId, CancellationToken ct)
    {
        User? userInfo = await userRepository.GetUserByIdAsync(userId, ct);

        if (userInfo is null)
            throw new NotFoundException($"The user ID: {userId} not exists.");

        logger.LogInformation("The user id '{UserId}' is retrieved", userId);
        return UserDto.FromEntity(userInfo);
    }

    public async Task<UserDto> GetUserByUsernameAsync(string username, CancellationToken ct)
    {
        var userInfo = await userRepository.GetByUsernameAsync(username, ct);

        if (userInfo is null)
            throw new NotFoundException($"The user by username '{username}' not exists.");

        logger.LogInformation("The user with username '{Username}' is retrieved", username);
        return UserDto.FromEntity(userInfo);
    }

    public async Task<List<UserDto>> GetAllUsersAsync(CancellationToken ct, int page = 1, int pageSize = 10)
    {
        List<User> users = await userRepository.GetAllUsersAsync(ct, page, pageSize);
        logger.LogInformation("The users retrieved in page {Page} with page size {PageSize}", page, pageSize);
        return UserDto.FromEntities(users);
    }

    public async Task<int> GetUsersCountAsync(CancellationToken ct)
    {
        int count = await userRepository.GetUsersCountAsync(ct);
        logger.LogInformation("The users count retrieved");
        return count;
    }

    public async Task<int> AddNewUserAsync(CreateUserDto userDto, CancellationToken ct)
    {
        if (await userRepository.IsUsernameExists(userDto.Username, ct))
            throw new ConflictException($"The user with username '{userDto.Username}' is already used.");

        userDto.Password = Hashing.HashPasswordOnly(userDto.Password);
        int newId = await userRepository.AddNewUserAsync(userDto, ct);

        if (newId == -1)
        {
            logger.LogError("Failed to add the user with username '{Username}' in the system.", userDto.Username);
            throw new InternalServerErrorException("Failed to add the user.");
        }

        logger.LogInformation("A new user with username '{Username}' was added successfully with ID {UserId}", userDto.Username, newId);
        return newId;
    }

    public async Task UpdateUserAsync(int userId, CreateUserDto userDto, CancellationToken ct)
    {
        var userInfo = await userRepository.GetUserByIdAsync(userId, ct);

        if (userInfo is null)
            throw new NotFoundException($"The user ID: {userId} not exists.");

        if (userInfo.Username != userDto.Username)
        {
            bool isUsernameUsed = await userRepository.IsUsernameExists(userDto.Username, ct);

            if (isUsernameUsed)
                throw new ConflictException($"The username: {userDto.Username} is already used.");
        }

        string userDtoPassword = Hashing.HashPasswordOnly(userDto.Password);

        userDto.Password = (userDtoPassword != userInfo.PasswordHash) ?
            Hashing.HashPasswordOnly(userDto.Password) :
            userDto.Password = userInfo.PasswordHash;

        bool isSuccess = await userRepository.UpdateUserAsync(userId, userDto, ct);

        if (!isSuccess)
        {
            logger.LogError("Failed to update the user with ID '{UserId}' and username '{Username}'.", userId, userDto.Username);
            throw new InternalServerErrorException("Failed to update the user.");
        }

        logger.LogInformation("The user with ID '{UserId}' was updated successfully", userId);
    }

    public async Task DeactivateUserAsync(int userId, CancellationToken ct)
    {
        bool isSuccess = await userRepository.DeactivateUserAsync(userId, ct);

        if (!isSuccess)
            throw new NotFoundException($"The user with ID: {userId} not exists.");

        logger.LogInformation("The user with ID '{UserId}' was deactivated", userId);
    }

    public async Task<bool> IsUserIdExists(int userId, CancellationToken ct)
    {
        bool isFound = await userRepository.IsUserIdExists(userId, ct);

        if (isFound)
            logger.LogDebug("The user with id '{UserId}' is found.", userId);
        else
            logger.LogDebug("The user with id '{UserId}' is not found.", userId);

        return isFound;
    }

    public async Task<bool> IsUsernameExists(string username, CancellationToken ct)
    {
        bool isFound = await userRepository.IsUsernameExists(username, ct);

        if (isFound)
            logger.LogDebug("The user with username '{Username}' is found.", username);
        else
            logger.LogDebug("The user with username '{Username}' is not found.", username);

        return isFound;
    }
}