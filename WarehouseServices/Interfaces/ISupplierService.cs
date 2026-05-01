using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;

namespace WarehouseServices.Interfaces;

public interface ISupplierService
{
    Task<SupplierDto> GetSupplierByIdAsync(int supplierId);
    Task<List<SupplierDto>> GetAllSuppliersAsync(int page = 1, int pageSize = 10);
    Task<int> GetSuppliersCountAsync();
    Task<int> AddNewSupplierAsync(CreateSupplierDto supplier);
    Task<bool> UpdateSupplierAsync(CreateSupplierDto supplier);
    Task<bool> DeleteSupplierAsync(int supplierId);
}
