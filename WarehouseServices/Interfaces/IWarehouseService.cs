using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;

namespace WarehouseServices.Interfaces;

public interface IWarehouseService
{
    Task<Warehouse> GetWarehouseByIdAsync(int customerId);
    Task<List<Warehouse>> GetAllWarehousesAsync(int page = 1, int pageSize = 10);
    Task<int> GetWarehousesCountAsync();
    Task<int> AddNewWarehouseAsync(CreateWarehouseDto customer);
    Task<bool> UpdateWarehouseAsync(CreateWarehouseDto customer);
    Task<bool> DeleteWarehouseAsync(int customerId);
}