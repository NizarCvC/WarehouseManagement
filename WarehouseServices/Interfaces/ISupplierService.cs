using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;

namespace WarehouseServices.Interfaces;

public interface ISupplierService
{
    Task<Supplier> GetSupplierByIdAsync(int supplierId);
    Task<List<Supplier>> GetAllSuppliersAsync(int page = 1, int pageSize = 10);
    Task<int> GetSuppliersCountAsync();
    Task<int> AddNewSupplierAsync(CreateSupplierDto supplier);
    Task<bool> UpdateSupplierAsync(CreateSupplierDto supplier);
    Task<bool> DeleteSupplierAsync(int supplierId);
}
