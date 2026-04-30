using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;

namespace WarehouseServices.Interfaces;

public interface IUserService
{
    Task<User> GetUserByIdAsync(int userId);
    Task<List<User>> GetAllUsersAsync(int page = 1, int pageSize = 10);
    Task<int> GetUsersCountAsync();
    Task<int> AddNewUserAsync(CreateUserDto user);
    Task<bool> UpdateUserAsync(CreateUserDto user);
    Task<bool> DeleteUserAsync(int userId);
}
