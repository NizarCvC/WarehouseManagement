using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;
using WarehouseCore.enums;
using WarehouseDataAccess.Interfaces;
using WarehouseServices.Exceptions;
using WarehouseServices.Interfaces;

namespace WarehouseServices.Services;

public class StockService(IStockRepository stockRepository,
    IProductService productService, IWarehouseService warehouseService,
    ILogger<StockService> logger) : IStockService
{
    public async Task<int> CreateStockTransferAsync(CreateStockTransferDto transfer, CancellationToken ct)
    {
        if (transfer.SourceWarehouseID == transfer.DestinationWarehouseID)
        {
            throw new BadRequestException("Source and destination warehouse IDs cannot be the same.");
        }

        WarehouseDto sourceWarehouse = await warehouseService.GetWarehouseByIdAsync(transfer.SourceWarehouseID, ct);

        if (!sourceWarehouse.IsActive)
        {
            throw new BadRequestException($"The source warehouse ID: {transfer.SourceWarehouseID} is not active.");
        }

        WarehouseDto destinationWarehouse = await warehouseService.GetWarehouseByIdAsync(transfer.DestinationWarehouseID, ct);

        if (!destinationWarehouse.IsActive)
        {
            throw new BadRequestException($"The destination warehouse ID: {transfer.DestinationWarehouseID} is not active.");
        }

        try
        {
            int newTransferId = await stockRepository.CreateStockTransferAsync(transfer, ct);
            logger.LogInformation("The stock transfer created from warehouse ID: {SourceWarehouseId} to warehouse ID: {DestinationWarehouseId}",
                transfer.SourceWarehouseID, transfer.DestinationWarehouseID);
            return newTransferId;
        }
        catch (SqlException ex) when (ex.Number == 50003)
        {
            throw new BadRequestException(ex.Message);
        }
        catch (SqlException ex) when (ex.Number == 50002)
        {
            throw new BadRequestException(ex.Message);
        }
    }

    public async Task<List<ItemLedgerDto>> GetItemLedgerAsync(int? productId, int? warehouseId, CancellationToken ct, int page = 1, int pageSize = 10)
    {
        await IsProductOrWarehouseExists(productId, warehouseId, ct);

        List<ItemLedgerDto> itemLedger = await stockRepository.GetItemLedgerAsync(productId, warehouseId, ct, page, pageSize);
        logger.LogInformation("The item ledger retrieved for product ID: {ProductId} and warehouse ID: {WarehouseId}", productId, warehouseId);
        return itemLedger;
    }

    public async Task<int> GetItemLedgerCountAsync(int? productId, int? warehouseId, CancellationToken ct)
    {
        int count = await stockRepository.GetItemLedgerCountAsync(productId, warehouseId, ct);
        logger.LogInformation("The item ledger count retrieved for product ID: {ProductId} and warehouse ID: {WarehouseId}", productId, warehouseId);
        return count;
    }

    public async Task<List<CurrentStockDto>> GetProductStockAsync(int productId, CancellationToken ct)
    {
        if (!await productService.IsProductExistsByIdAsync(productId, ct))
        {
            throw new NotFoundException($"The product ID: {productId} not exists.");
        }

        List<CurrentStockDto> productStock = await stockRepository.GetProductStockAsync(productId, ct);
        logger.LogInformation("The product stock retrieved for product ID: {ProductId}", productId);
        return productStock;
    }

    public async Task<CurrentStockDto> GetProductStockInWarehouseAsync(int productId, int warehouseId, CancellationToken ct)
    {
        await IsProductOrWarehouseExists(productId, warehouseId, ct);

        CurrentStockDto? currentStock = await stockRepository.GetProductStockInWarehouseAsync(productId, warehouseId, ct);

        if (currentStock == null)
        {
            logger.LogInformation("No stock found for product ID: {ProductId} in warehouse ID: {WarehouseId}", productId, warehouseId);
            return new CurrentStockDto
            {
                ProductID = productId,
                ProductName = string.Empty,
                WarehouseName = string.Empty,
                Quantity = 0,
                UnitCost = 0,
                TotalValue = 0
            };
        }

        logger.LogInformation("The product stock retrieved for product ID: {ProductId} in warehouse ID: {WarehouseId}", productId, warehouseId);
        return currentStock;
    }

    public async Task<List<CurrentStockDto>> GetWarehouseStockAsync(int warehouseId, CancellationToken ct, int page = 1, int pageSize = 10)
    {
        if (!await warehouseService.IsWarehouseExistsByIdAsync(warehouseId, ct))
        {
            throw new NotFoundException($"The warehouse ID: {warehouseId} not exists.");
        }

        List<CurrentStockDto> warehouseStock = await stockRepository.GetWarehouseStockAsync(warehouseId, ct, page, pageSize);
        logger.LogInformation("The warehouse stock retrieved for warehouse ID: {WarehouseId} in page {Page} with page size {PageSize}", warehouseId, page, pageSize);
        return warehouseStock;
    }

    public async Task<int> GetWarehouseStockCountAsync(int warehouseId, CancellationToken ct)
    {
        int count = await stockRepository.GetWarehouseStockCountAsync(warehouseId, ct);
        logger.LogInformation("The warehouse stock count retrieved for warehouse ID: {WarehouseId}", warehouseId);
        return count;
    }

    public async Task UpdateTransferStatusAsync(int transferId, int newStatusId, CancellationToken ct)
    {
        StockTransfer? transfer = await stockRepository.GetStockTransferByIdAsync(transferId, ct);

        if (transfer is null)
            throw new NotFoundException($"The stock transfer ID: {transferId} not exists.");

        if (transfer.TransferStatus == enTransferStatus.Completed || transfer.TransferStatus == enTransferStatus.Cancelled)
            throw new BadRequestException($"The stock transfer ID: {transferId} is already completed or cancelled and cannot be updated.");

        if (!Enum.IsDefined(typeof(enTransferStatus), newStatusId))
            throw new NotFoundException($"The stock transfer status ID: {newStatusId} not exists.");

        await stockRepository.UpdateTransferStatusAsync(transferId, newStatusId, ct);
        logger.LogInformation("The stock transfer ID: {TransferId} status updated to {New StatusId}", transferId, newStatusId);
    }

    private async Task<bool> IsProductOrWarehouseExists(int? productId, int? warehouseId, CancellationToken ct)
    {
        if (productId is not null)
        {
            if (!await productService.IsProductExistsByIdAsync(productId.Value, ct))
                throw new NotFoundException($"The product ID: {productId} not exists.");
        }

        if (warehouseId is not null)
        {
            if (!await warehouseService.IsWarehouseExistsByIdAsync(warehouseId.Value, ct))
                throw new NotFoundException($"The warehouse ID: {warehouseId} not exists.");
        }

        return true;
    }
}
