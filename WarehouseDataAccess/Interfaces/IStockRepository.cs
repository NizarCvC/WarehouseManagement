using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;

namespace WarehouseDataAccess.Interfaces;

public interface IStockRepository
{
    Task<bool> ExecuteStockTransferAsync(CreateStockTransferDto transfer, CancellationToken ct);
    Task<List<CurrentStockDto>> GetCurrentStockAsync(int productId, CancellationToken ct);
    Task<List<CurrentStockDto>> GetCurrentStockAsync(int productId, int warehouseId, CancellationToken ct);
    Task<List<ItemLedgerDto>> GetItemLedgerAsync(int? productId, int? warehouseId, CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetItemLedgerCountAsync(int? productId, int? warehouseId, CancellationToken ct);
}
