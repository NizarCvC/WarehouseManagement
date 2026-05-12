using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;

namespace WarehouseServices.Interfaces;

public interface ISupplierService
{
    Task<SupplierDto?> GetSupplierByIdAsync(int supplierId, CancellationToken ct);
    Task<List<SupplierDto>> GetAllSuppliersAsync(CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetSuppliersCountAsync(CancellationToken ct);
    Task<int> AddNewSupplierAsync(CreateSupplierDto supplierDto, CancellationToken ct);
    Task UpdateSupplierAsync(int supplierId, CreateSupplierDto supplierDto, CancellationToken ct);
    Task DeactivateSupplierAsync(int supplierId, CancellationToken ct);
}
