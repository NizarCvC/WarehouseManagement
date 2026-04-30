using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;

namespace WarehouseServices.Interfaces;

public interface ICustomerService
{
    Task<Customer> GetCustomerByIdAsync(int customerId);
    Task<List<Customer>> GetAllCustomersAsync(int page = 1, int pageSize = 10);
    Task<int> GetCustomersCountAsync();
    Task<int> AddNewCustomerAsync(CreateCustomerDto customer);
    Task<bool> UpdateCustomerAsync(CreateCustomerDto customer);
    Task<bool> DeleteCustomerAsync(int customerId);
}