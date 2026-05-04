using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;

namespace WarehouseServices.Interfaces;

public interface ICustomerService
{
    Task<CustomerDto?> GetCustomerByIdAsync(int customerId, CancellationToken ct);
    Task<List<CustomerDto>> GetAllCustomersAsync(CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetCustomersCountAsync(CancellationToken ct);
    Task<int> AddNewCustomerAsync(CreateCustomerDto customer, CancellationToken ct);
    Task<bool> UpdateCustomerAsync(int customerId, CreateCustomerDto customer, CancellationToken ct);
    Task<bool> DeleteCustomerAsync(int customerId, CancellationToken ct);
}