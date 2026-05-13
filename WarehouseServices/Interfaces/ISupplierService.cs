using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;

namespace WarehouseServices.Interfaces;

public interface ISupplierService
{
    Task<SupplierDto> GetSupplierByIdAsync(int supplierId, CancellationToken ct);
    Task<SupplierDto> GetSupplierByEmailAsync(string email, CancellationToken ct);
    Task<SupplierDto> GetSupplierByPhoneAsync(string phone, CancellationToken ct);
    Task<SupplierDto> GetSupplierByTaxNumberAsync(string taxNumber, CancellationToken ct);
    Task<List<SupplierDto>> GetAllSuppliersAsync(CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetSuppliersCountAsync(CancellationToken ct);
    Task<int> AddNewSupplierAsync(CreateSupplierDto supplierDto, CancellationToken ct);
    Task UpdateSupplierAsync(int supplierId, CreateSupplierDto supplierDto, CancellationToken ct);
    Task DeactivateSupplierAsync(int supplierId, CancellationToken ct);
    Task<bool> IsSupplierExistsByIdAsync(int supplierId, CancellationToken ct);
    Task<bool> IsSupplierExistsByEmailAsync(string email, CancellationToken ct);
    Task<bool> IsSupplierExistsByPhoneAsync(string phone, CancellationToken ct);
    Task<bool> IsSupplierExistsByTaxNumberAsync(string taxNumber, CancellationToken ct);
}