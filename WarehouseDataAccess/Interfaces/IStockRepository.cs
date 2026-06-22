using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;

namespace WarehouseDataAccess.Interfaces;

public interface IStockRepository
{
    Task<int> CreateStockTransferAsync(CreateStockTransferDto transfer, CancellationToken ct);
    Task<StockTransfer?> GetStockTransferByIdAsync(int transferId, CancellationToken ct);
    Task<bool> UpdateTransferStatusAsync(int transferId, int newStatusId, CancellationToken ct);
    Task<CurrentStockDto?> GetProductStockInWarehouseAsync(int productId, int warehouseId, CancellationToken ct);
    Task<List<CurrentStockDto>> GetProductStockAsync(int productId, CancellationToken ct);
    Task<List<CurrentStockDto>> GetWarehouseStockAsync(int warehouseId, CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetWarehouseStockCountAsync(int warehouseId, CancellationToken ct);
    Task<List<ItemLedgerDto>> GetItemLedgerAsync(int? productId, int? warehouseId, CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetItemLedgerCountAsync(int? productId, int? warehouseId, CancellationToken ct);
}