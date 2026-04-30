using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;

namespace WarehouseDataAccess.Interfaces;

public interface IUserRepository
{
    public Task<User> GetUserByIdAsync(int userId);
    public Task<List<User>> GetAllUsersAsync(int page = 1, int pageSize = 10);
    public Task<int> GetUsersCountAsync();
    public Task<int> AddNewUserAsync(CreateUserDto user);
    public Task<bool> UpdateUserAsync(CreateUserDto user);
    public Task<bool> DeleteUserAsync(int userId);
}
