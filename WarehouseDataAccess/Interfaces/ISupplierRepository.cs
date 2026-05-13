using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;

namespace WarehouseDataAccess.Interfaces;

public interface ISupplierRepository
{
    Task<Supplier?> GetSupplierByIdAsync(int supplierId, CancellationToken ct);
    Task<Supplier?> GetSupplierByEmailAsync(string email, CancellationToken ct);
    Task<Supplier?> GetSupplierByPhoneAsync(string phone, CancellationToken ct);
    Task<Supplier?> GetSupplierByTaxNumberAsync(string taxNumber, CancellationToken ct);
    Task<List<Supplier>> GetAllSuppliersAsync(CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetSuppliersCountAsync(CancellationToken ct);
    Task<int> AddNewSupplierAsync(CreateSupplierDto supplier, CancellationToken ct);
    Task<bool> UpdateSupplierAsync(int supplierId, CreateSupplierDto supplier, CancellationToken ct);
    Task<bool> DeactivateSupplierAsync(int supplierId, CancellationToken ct);
    Task<bool> IsSupplierExistsByIdAsync(int supplierId, CancellationToken ct);
    Task<bool> IsSupplierExistsByEmailAsync(string email, CancellationToken ct);
    Task<bool> IsSupplierExistsByPhoneAsync(string phone, CancellationToken ct);
    Task<bool> IsSupplierExistsByTaxNumberAsync(string taxNumber, CancellationToken ct);
}