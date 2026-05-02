using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;

namespace WarehouseServices.Interfaces;

public interface IWarehouseService
{
    Task<WarehouseDto> GetWarehouseByIdAsync(int customerId, CancellationToken ct);
    Task<List<WarehouseDto>> GetAllWarehousesAsync(CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetWarehousesCountAsync(CancellationToken ct);
    Task<int> AddNewWarehouseAsync(CreateWarehouseDto customer, CancellationToken ct);
    Task<bool> UpdateWarehouseAsync(CreateWarehouseDto customer, CancellationToken ct);
    Task<bool> DeleteWarehouseAsync(int customerId, CancellationToken ct);
}