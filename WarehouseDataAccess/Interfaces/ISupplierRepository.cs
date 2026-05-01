using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;

namespace WarehouseDataAccess.Interfaces;

public interface ISupplierRepository
{
    Task<Supplier?> GetSupplierByIdAsync(int supplierId);
    Task<List<Supplier>> GetAllSuppliersAsync(int page = 1, int pageSize = 10);
    Task<int> GetSuppliersCountAsync();
    Task<int> AddNewSupplierAsync(CreateSupplierDto supplier);
    Task<bool> UpdateSupplierAsync(int supplierId, CreateSupplierDto supplier);
    Task<bool> DeleteSupplierAsync(int supplierId);
}
