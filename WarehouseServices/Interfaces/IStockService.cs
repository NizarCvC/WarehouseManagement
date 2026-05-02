using WarehouseCore.CustomTypes;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;

namespace WarehouseServices.Interfaces;

public interface IStockService
{
    Task<bool> ExecuteStockTransferAsync(StockTransfer transfer, List<TransferItemType> items, CancellationToken ct);
    Task<List<CurrentStockDto>> GetCurrentStockAsync(int productId, CancellationToken ct);
    Task<List<CurrentStockDto>> GetCurrentStockAsync(int productId, int warehouseId, CancellationToken ct);
    Task<List<ItemLedgerDto>> GetItemLedgerAsync(int? productId, int? warehouseId, CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetItemLedgerCountAsync(int? productId, int? warehouseId, CancellationToken ct);
}
