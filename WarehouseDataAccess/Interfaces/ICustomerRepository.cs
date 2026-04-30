using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;

namespace WarehouseDataAccess.Interfaces;

public interface ICustomerRepository
{
    public Task<Customer> GetCustomerByIdAsync(int customerId);
    public Task<List<Customer>> GetAllCustomersAsync(int page = 1, int pageSize = 10);
    public Task<int> GetCustomersCountAsync();
    public Task<int> AddNewCustomerAsync(CreateCustomerDto customer);
    public Task<bool> UpdateCustomerAsync(CreateCustomerDto customer);
    public Task<bool> DeleteCustomerAsync(int customerId);
}
