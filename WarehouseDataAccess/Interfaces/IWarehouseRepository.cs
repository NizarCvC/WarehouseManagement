using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;

namespace WarehouseDataAccess.Interfaces;

public interface IWarehouseRepository
{
    public Task<Warehouse> GetWarehouseByIdAsync(int customerId);
    public Task<List<Warehouse>> GetAllWarehousesAsync(int page = 1, int pageSize = 10);
    public Task<int> GetWarehousesCountAsync();
    public Task<int> AddNewWarehouseAsync(CreateWarehouseDto customer);
    public Task<bool> UpdateWarehouseAsync(CreateWarehouseDto customer);
    public Task<bool> DeleteWarehouseAsync(int customerId);
}
