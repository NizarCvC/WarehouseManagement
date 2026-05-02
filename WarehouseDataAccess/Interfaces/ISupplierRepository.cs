using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;

namespace WarehouseDataAccess.Interfaces;

public interface ISupplierRepository
{
    Task<Supplier?> GetSupplierByIdAsync(int supplierId, CancellationToken ct);
    Task<List<Supplier>> GetAllSuppliersAsync(CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetSuppliersCountAsync(CancellationToken ct);
    Task<int> AddNewSupplierAsync(CreateSupplierDto supplier, CancellationToken ct);
    Task<bool> UpdateSupplierAsync(int supplierId, CreateSupplierDto supplier, CancellationToken ct);
    Task<bool> DeleteSupplierAsync(int supplierId, CancellationToken ct);
}
