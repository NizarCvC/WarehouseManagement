using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
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

    public async Task<int> CreateStockTransferAsync(CreateStockTransferDto transfer, CancellationToken ct)
    {
        int newStockTransferID = -1;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand("sp_ExecuteStockTransfer", connection))
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@TransferNumber", SqlDbType.VarChar) { Value = transfer.TransferNumber });
            command.Parameters.Add(new SqlParameter("@SourceWarehouseID", SqlDbType.Int) { Value = transfer.SourceWarehouseID });
            command.Parameters.Add(new SqlParameter("@DestinationWarehouseID", SqlDbType.Int) { Value = transfer.DestinationWarehouseID });
            command.Parameters.Add(new SqlParameter("@CreatedByID", SqlDbType.Int) { Value = transfer.CreatedByID });
            command.Parameters.Add(new SqlParameter("@Note", SqlDbType.NVarChar) { Value = (object?)transfer.Note ?? DBNull.Value });

            var records = CreateTransferItemRecords(transfer.Items);

            SqlParameter itemsParam = new SqlParameter("@Items", SqlDbType.Structured)
            {
                TypeName = "dbo.InvoiceItemType",
                Value = records
            };
            command.Parameters.Add(itemsParam);

            connection.Open();

            object result = await command.ExecuteScalarAsync(ct);
            if (result != null && result != DBNull.Value)
                newStockTransferID = Convert.ToInt32(result);

            return newStockTransferID;
        }
    }

    public async Task<List<ItemLedgerDto>> GetItemLedgerAsync(int? productId, int? warehouseId, CancellationToken ct, int page = 1, int pageSize = 10)
    {
        List<ItemLedgerDto> itemLedgers = new List<ItemLedgerDto>();

        string query = ItemLedgerQuery(productId, warehouseId);

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            AddItemLedgerParameters(command, productId, warehouseId);
            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    ItemLedgerDto itemLedger = MapReaderToItemLedger(reader);
                    itemLedgers.Add(itemLedger);
                }

                return itemLedgers;
            }
        }
    }

    public async Task<int> GetItemLedgerCountAsync(int? productId, int? warehouseId, CancellationToken ct)
    {
        string query = ItemLedgerCountQuery(productId, warehouseId);
        int countNumber = 0;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            AddItemLedgerParameters(command, productId, warehouseId);
            await connection.OpenAsync(ct);

            object result = await command.ExecuteScalarAsync(ct);

            if (result != null && result != DBNull.Value)
                countNumber = Convert.ToInt32(result);

            return countNumber;
        }
    }

    public async Task<List<CurrentStockDto>> GetProductStockAsync(int productId, CancellationToken ct)
    {
        List<CurrentStockDto> currentStocks = new List<CurrentStockDto>();

        string query = @"SELECT cs.ProductID, cs.ProductName, cs.WarehouseName, cs.Quantity, cs.UnitCost,
                        cs.TotalValue FROM vw_CurrentStock cs
                        WHERE cs.ProductID = @ProductId";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int) { Value = productId });
            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    CurrentStockDto currentStock = MapReaderToCurrentStock(reader);
                    currentStocks.Add(currentStock);
                }

                return currentStocks;
            }
        }
    }

    public async Task<CurrentStockDto?> GetProductStockInWarehouseAsync(int productId, int warehouseId, CancellationToken ct)
    {
        string query = @"SELECT cs.ProductID, cs.ProductName, cs.WarehouseName, cs.Quantity, cs.UnitCost, cs.TotalValue 
                        FROM vw_CurrentStock cs INNER JOIN Warehouses w ON cs.WarehouseName = w.Name
                        WHERE cs.ProductID = @ProductID AND w.WarehouseID = @WarehouseID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int) { Value = productId });
            command.Parameters.Add(new SqlParameter("@WarehouseID", SqlDbType.Int) { Value = warehouseId });
            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                if (await reader.ReadAsync(ct))
                    return MapReaderToCurrentStock(reader);
                else
                    return null;
            }
        }
    }

    public async Task<List<CurrentStockDto>> GetWarehouseStockAsync(int warehouseId, CancellationToken ct, int page = 1, int pageSize = 10)
    {
        List<CurrentStockDto> currentStocks = new List<CurrentStockDto>();

        string query = @"SELECT cs.ProductID, cs.ProductName, cs.WarehouseName, cs.Quantity, cs.UnitCost, cs.TotalValue 
                        FROM vw_CurrentStock cs INNER JOIN Warehouses w ON cs.WarehouseName = w.Name
                        WHERE w.WarehouseID = @WarehouseID
                        ORDER BY cs.ProductID
                        OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
                        FETCH NEXT @RowsPerPage ROWS ONLY;";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@WarehouseID", SqlDbType.Int) { Value = warehouseId });
            command.Parameters.Add(new SqlParameter("@PageNumber", SqlDbType.Int) { Value = page });
            command.Parameters.Add(new SqlParameter("@RowsPerPage", SqlDbType.Int) { Value = pageSize });
            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    CurrentStockDto currentStock = MapReaderToCurrentStock(reader);
                    currentStocks.Add(currentStock);
                }

                return currentStocks;
            }
        }
    }

    public async Task<int> GetWarehouseStockCountAsync(int warehouseId, CancellationToken ct)
    {
        string query = @"SELECT COUNT(*) FROM vw_CurrentStock cs 
                        INNER JOIN Warehouses w ON cs.WarehouseName = w.Name
                        WHERE w.WarehouseID = @WarehouseID";

        int countNumber = 0;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@WarehouseID", SqlDbType.Int) { Value = warehouseId });
            await connection.OpenAsync(ct);

            object result = await command.ExecuteScalarAsync(ct);

            if (result != null && result != DBNull.Value)
                countNumber = Convert.ToInt32(result);

            return countNumber;
        }
    }

    public async Task<bool> UpdateTransferStatusAsync(int transferId, int newStatusId, CancellationToken ct)
    {
        string query = @"UPDATE StockTransfers SET StatusID = @NewStatusId
                        WHERE StockTransferID = @StockTransferID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@NewStatusId", SqlDbType.Int) { Value = newStatusId });
            command.Parameters.Add(new SqlParameter("@StockTransferID", SqlDbType.Int) { Value = transferId });
            await connection.OpenAsync(ct);

            int rowsAffected = await command.ExecuteNonQueryAsync(ct);

            return rowsAffected > 0;
        }
    }

    private CurrentStockDto MapReaderToCurrentStock(SqlDataReader reader)
    {
        return new CurrentStockDto
        {
            ProductID = reader.GetInt32("ProductID"),
            ProductName = reader.GetString("ProductName"),
            WarehouseName = reader.GetString("WarehouseName"),
            Quantity = reader.GetDecimal("Quantity"),
            UnitCost = reader.GetDecimal("UnitCost"),
            TotalValue = reader.GetDecimal("TotalValue")
        };
    }

    private ItemLedgerDto MapReaderToItemLedger(SqlDataReader reader)
    {
        return new ItemLedgerDto
        {
            InventoryTransactionID = reader.GetInt32("InventoryTransactionID"),
            TransactionDate = reader.GetDateTime("TransactionDate"),
            ProductName = reader.GetString("ProductName"),
            WarehouseName = reader.GetString("WarehouseName"),
            TransactionType = reader.GetString("TransactionType"),
            MovementType = reader.GetString("MovementType"),
            Quantity = reader.GetDecimal("Quantity"),
            ReferenceDocument = reader.IsDBNull(reader.GetOrdinal("ReferenceDocument")) ?
                null : reader.GetString("ReferenceDocument"),
            CreatedByName = reader.GetString("CreatedByName")
        };
    }

    private IEnumerable<SqlDataRecord> CreateTransferItemRecords(List<CreateTransferItemDto> items)
    {
        SqlMetaData[] schema = [
            new SqlMetaData("ProductID", SqlDbType.Int),
            new SqlMetaData("Quantity", SqlDbType.Decimal, 18, 2)
        ];

        foreach (var item in items)
        {
            SqlDataRecord record = new SqlDataRecord(schema);
            record.SetInt32(0, item.ProductID);
            record.SetDecimal(1, item.Quantity);

            yield return record;
        }
    }

    private string ItemLedgerQuery(int? productId, int? warehouseId)
    {
        if (productId is null && warehouseId is null)
        {
            return @"SELECT il.InventoryTransactionID, il.TransactionDate, il.ProductName, il.WarehouseName,
                    il.TransactionType, il.MovementType, il.Quantity, il.ReferenceDocument,
                    il.CreatedByName FROM vw_ItemLedger il";
        }
        else if (productId is not null && warehouseId is null)
        {
            return @"SELECT il.InventoryTransactionID, il.TransactionDate, il.ProductName, il.WarehouseName,
                    il.TransactionType, il.MovementType, il.Quantity, il.ReferenceDocument, il.CreatedByName 
                    FROM vw_ItemLedger il INNER JOIN InventoryTransactions it ON it.InventoryTransactionID = il.InventoryTransactionID
                    WHERE it.ProductID = @ProductID";
        }
        else if (productId is null && warehouseId is not null)
        {
            return @"SELECT il.InventoryTransactionID, il.TransactionDate, il.ProductName, il.WarehouseName,
                    il.TransactionType, il.MovementType, il.Quantity, il.ReferenceDocument, il.CreatedByName 
                    FROM vw_ItemLedger il INNER JOIN InventoryTransactions it ON it.InventoryTransactionID = il.InventoryTransactionID
                    WHERE it.WarehouseID = @WarehouseID";
        }
        else
        {
            return @"SELECT il.InventoryTransactionID, il.TransactionDate, il.ProductName, il.WarehouseName,
                    il.TransactionType, il.MovementType, il.Quantity, il.ReferenceDocument, il.CreatedByName 
                    FROM vw_ItemLedger il INNER JOIN InventoryTransactions it ON it.InventoryTransactionID = il.InventoryTransactionID
                    WHERE it.WarehouseID = @WarehouseID AND it.ProductID = @ProductID";
        }
    }

    private string ItemLedgerCountQuery(int? productId, int? warehouseId)
    {
        if (productId is null && warehouseId is null)
        {
            return @"SELECT COUNT(*) FROM vw_ItemLedger il";
        }
        else if (productId is not null && warehouseId is null)
        {
            return @"SELECT COUNT(*) FROM vw_ItemLedger il INNER JOIN InventoryTransactions it
                    ON it.InventoryTransactionID = il.InventoryTransactionID
                    WHERE it.ProductID = @ProductID";
        }
        else if (productId is null && warehouseId is not null)
        {
            return @"SELECT COUNT(*) FROM vw_ItemLedger il INNER JOIN InventoryTransactions it
                    ON it.InventoryTransactionID = il.InventoryTransactionID
                    WHERE it.WarehouseID = @WarehouseID";
        }
        else
        {
            return @"SELECT COUNT(*) FROM vw_ItemLedger il INNER JOIN InventoryTransactions it 
                    ON it.InventoryTransactionID = il.InventoryTransactionID
                    WHERE it.WarehouseID = @WarehouseID AND it.ProductID = @ProductID";
        }
    }

    private void AddItemLedgerParameters(SqlCommand command, int? productId, int? warehouseId)
    {
        if (productId is not null && warehouseId is not null)
        {
            command.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int) { Value = productId });
            command.Parameters.Add(new SqlParameter("@WarehouseID", SqlDbType.Int) { Value = warehouseId });
        }
        else if (productId is not null && warehouseId is null)
        {
            command.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int) { Value = productId });
        }
        else if (productId is null && warehouseId is not null)
        {
            command.Parameters.Add(new SqlParameter("@WarehouseID", SqlDbType.Int) { Value = warehouseId });
        }
    }
}
