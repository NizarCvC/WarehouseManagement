using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;

namespace WarehouseDataAccess.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetCustomerByIdAsync(int customerId);
    Task<List<Customer>> GetAllCustomersAsync(int page = 1, int pageSize = 10);
    Task<int> GetCustomersCountAsync();
    Task<int> AddNewCustomerAsync(CreateCustomerDto customer);
    Task<bool> UpdateCustomerAsync(int customerId, CreateCustomerDto customer);
    Task<bool> DeleteCustomerAsync(int customerId);
}
