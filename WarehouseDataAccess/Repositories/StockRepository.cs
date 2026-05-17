using Microsoft.Extensions.Configuration;
using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseDataAccess.Interfaces;

namespace WarehouseDataAccess.Repositories;

public class StockRepository : IStockRepository
{

    private readonly string _connectionString;

    public StockRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public Task<int> CreateStockTransferAsync(CreateStockTransferDto transfer, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<List<ItemLedgerDto>> GetItemLedgerAsync(int? productId, int? warehouseId, CancellationToken ct, int page = 1, int pageSize = 10)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetItemLedgerCountAsync(int? productId, int? warehouseId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<List<CurrentStockDto>> GetProductStockAsync(int productId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<CurrentStockDto?> GetProductStockInWarehouseAsync(int productId, int warehouseId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<List<CurrentStockDto>> GetWarehouseStockAsync(int warehouseId, CancellationToken ct, int page = 1, int pageSize = 10)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetWarehouseStockCountAsync(int warehouseId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateTransferStatusAsync(int transferId, int newStatusId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
