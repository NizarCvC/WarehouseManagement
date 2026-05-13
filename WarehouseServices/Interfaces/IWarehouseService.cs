using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;

namespace WarehouseServices.Interfaces;

public interface IWarehouseService
{
    Task<WarehouseDto> GetWarehouseByIdAsync(int warehouseId, CancellationToken ct);
    Task<WarehouseDto> GetWarehouseByNameAsync(string name, CancellationToken ct);
    Task<WarehouseDto> GetWarehouseByCodeAsync(string code, CancellationToken ct);
    Task<List<WarehouseDto>> GetAllWarehousesAsync(CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetWarehousesCountAsync(CancellationToken ct);
    Task<int> AddNewWarehouseAsync(CreateWarehouseDto warehouseDto, CancellationToken ct);
    Task UpdateWarehouseAsync(int warehouseId, CreateWarehouseDto warehouseDto, CancellationToken ct);
    Task DeactivateWarehouseAsync(int warehouseId, CancellationToken ct);
    Task<bool> IsWarehouseExistsByIdAsync(int warehouseId, CancellationToken ct);
    Task<bool> IsWarehouseExistsByNameAsync(string name, CancellationToken ct);
    Task<bool> IsWarehouseExistsByCodeAsync(string code, CancellationToken ct);
}