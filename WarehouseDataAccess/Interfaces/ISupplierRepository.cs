using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;

namespace WarehouseDataAccess.Interfaces;

public interface ISupplierRepository
{
    public Task<Supplier> GetSupplierByIdAsync(int supplierId);
    public Task<List<Supplier>> GetAllSuppliersAsync(int page = 1, int pageSize = 10);
    public Task<int> GetSuppliersCountAsync();
    public Task<int> AddNewSupplierAsync(CreateSupplierDto supplier);
    public Task<bool> UpdateSupplierAsync(CreateSupplierDto supplier);
    public Task<bool> DeleteSupplierAsync(int supplierId);
}
