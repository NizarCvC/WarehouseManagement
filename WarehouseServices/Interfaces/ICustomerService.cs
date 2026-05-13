using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;

namespace WarehouseServices.Interfaces;

public interface ICustomerService
{
    Task<CustomerDto> GetCustomerByIdAsync(int customerId, CancellationToken ct);
    Task<List<CustomerDto>> GetAllCustomersAsync(CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetCustomersCountAsync(CancellationToken ct);
    Task<int> AddNewCustomerAsync(CreateCustomerDto customerDto, CancellationToken ct);
    Task UpdateCustomerAsync(int customerId, CreateCustomerDto customerDto, CancellationToken ct);
    Task DeactivateCustomerAsync(int customerId, CancellationToken ct);
}