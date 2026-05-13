using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;

namespace WarehouseDataAccess.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetCustomerByIdAsync(int customerId, CancellationToken ct);
    Task<Customer?> GetCustomerByEmailAsync(string email, CancellationToken ct);
    Task<Customer?> GetCustomerByPhoneAsync(string phone, CancellationToken ct);
    Task<List<Customer>> GetAllCustomersAsync(CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetCustomersCountAsync(CancellationToken ct);
    Task<int> AddNewCustomerAsync(CreateCustomerDto customer, CancellationToken ct);
    Task<bool> UpdateCustomerAsync(int customerId, CreateCustomerDto customer, CancellationToken ct);
    Task<bool> DeactivateCustomerAsync(int customerId, CancellationToken ct);
    Task<bool> IsCustomerExistsByIdAsync(int customerId, CancellationToken ct);
    Task<bool> IsCustomerExistsByEmailAsync(string email, CancellationToken ct);
    Task<bool> IsCustomerExistsByPhoneAsync(string phone, CancellationToken ct);
}
