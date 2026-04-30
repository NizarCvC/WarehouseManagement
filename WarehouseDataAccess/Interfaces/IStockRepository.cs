using WarehouseCore.CustomTypes;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;

namespace WarehouseDataAccess.Interfaces;

public interface IStockRepository
{
    Task<bool> ExecuteStockTransferAsync(StockTransfer transfer, List<TransferItemType> items);
    Task<List<CurrentStockDto>> GetCurrentStockAsync(int productId);
    Task<List<CurrentStockDto>> GetCurrentStockAsync(int productId, int warehouseId);
    Task<List<ItemLedgerDto>> GetItemLedgerAsync(int? productId, int? warehouseId, int page = 1, int pageSize = 10);
    public Task<int> GetItemLedgerCountAsync(int? productId, int? warehouseId);
}
