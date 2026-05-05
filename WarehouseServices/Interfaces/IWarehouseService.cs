using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;

namespace WarehouseServices.Interfaces;

public interface IWarehouseService
{
    Task<WarehouseDto?> GetWarehouseByIdAsync(int warehouseId, CancellationToken ct);
    Task<List<WarehouseDto>> GetAllWarehousesAsync(CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetWarehousesCountAsync(CancellationToken ct);
    Task<int> AddNewWarehouseAsync(CreateWarehouseDto warehouseDto, CancellationToken ct);
    Task<bool> UpdateWarehouseAsync(int warehouseId, CreateWarehouseDto warehouseDto, CancellationToken ct);
    Task<bool> DeleteWarehouseAsync(int warehouseId, CancellationToken ct);
}