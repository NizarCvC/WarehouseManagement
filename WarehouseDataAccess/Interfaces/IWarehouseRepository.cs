using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;

namespace WarehouseDataAccess.Interfaces;

public interface IWarehouseRepository
{
    Task<Warehouse?> GetWarehouseByIdAsync(int warehouseId, CancellationToken ct);
    Task<Warehouse?> GetWarehouseByNameAsync(string name, CancellationToken ct);
    Task<Warehouse?> GetWarehouseByCodeAsync(string code, CancellationToken ct);
    Task<List<Warehouse>> GetAllWarehousesAsync(CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetWarehousesCountAsync(CancellationToken ct);
    Task<int> AddNewWarehouseAsync(CreateWarehouseDto warehouse, CancellationToken ct);
    Task<bool> UpdateWarehouseAsync(int warehouseId, CreateWarehouseDto warehouse, CancellationToken ct);
    Task<bool> DeactivateWarehouseAsync(int warehouseId, CancellationToken ct);
    Task<bool> IsWarehouseExistsByIdAsync(int warehouseId, CancellationToken ct);
    Task<bool> IsWarehouseExistsByNameAsync(string name, CancellationToken ct);
    Task<bool> IsWarehouseExistsByCodeAsync(string code, CancellationToken ct);
}
