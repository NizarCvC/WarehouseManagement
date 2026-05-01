using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;

namespace WarehouseDataAccess.Interfaces;

public interface IWarehouseRepository
{
    Task<Warehouse?> GetWarehouseByIdAsync(int warehouseId);
    Task<List<Warehouse>> GetAllWarehousesAsync(int page = 1, int pageSize = 10);
    Task<int> GetWarehousesCountAsync();
    Task<int> AddNewWarehouseAsync(CreateWarehouseDto warehouse);
    Task<bool> UpdateWarehouseAsync(int warehouseId, CreateWarehouseDto warehouse);
    Task<bool> DeleteWarehouseAsync(int warehouseId);
}
