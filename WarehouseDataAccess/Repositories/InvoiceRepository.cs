using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
using Microsoft.Extensions.Configuration;
using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;
using WarehouseDataAccess.Interfaces;

namespace WarehouseDataAccess.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly string _connectionString;

    public InvoiceRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<int> CreatePurchaseInvoiceAsync(CreatePurchaseInvoiceDto purchaseInvoiceDto, CancellationToken ct)
    {
        int newInvoiceId = -1;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand("sp_ApprovePurchaseInvoice", connection))
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@InvoiceNumber", SqlDbType.VarChar) { Value = purchaseInvoiceDto.InvoiceNumber });
            command.Parameters.Add(new SqlParameter("@SupplierID", SqlDbType.Int) { Value = purchaseInvoiceDto.SupplierID });
            command.Parameters.Add(new SqlParameter("@WarehouseID", SqlDbType.Int) { Value = purchaseInvoiceDto.WarehouseID });
            command.Parameters.Add(new SqlParameter("@CreatedByID", SqlDbType.Int) { Value = purchaseInvoiceDto.CreatedByID });
            command.Parameters.Add(new SqlParameter("@Subtotal", SqlDbType.Decimal) { Value = purchaseInvoiceDto.Subtotal });
            command.Parameters.Add(new SqlParameter("@DiscountAmount", SqlDbType.Decimal) { Value = purchaseInvoiceDto.DiscountAmount });
            command.Parameters.Add(new SqlParameter("@TaxAmount", SqlDbType.Decimal) { Value = purchaseInvoiceDto.TaxAmount });
            command.Parameters.Add(new SqlParameter("@Note", SqlDbType.NVarChar) { Value = (object?)purchaseInvoiceDto.Note ?? DBNull.Value });

            var records = CreateInvoiceItemRecords(purchaseInvoiceDto.Items);

            SqlParameter itemsParam = new SqlParameter("@Items", SqlDbType.Structured)
            {
                TypeName = "dbo.InvoiceItemType",
                Value = records
            };
            command.Parameters.Add(itemsParam);

            await connection.OpenAsync(ct);

            object result = await command.ExecuteScalarAsync(ct);
            if (result != null && result != DBNull.Value)
                newInvoiceId = Convert.ToInt32(result);

            return newInvoiceId;
        }
    }

    public async Task<int> CreateSalesInvoiceAsync(CreateSalesInvoiceDto salesInvoiceDto, CancellationToken ct)
    {
        int newInvoiceId = -1;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand("sp_ApproveSalesInvoice", connection))
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@InvoiceNumber", SqlDbType.VarChar) { Value = salesInvoiceDto.InvoiceNumber });
            command.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.Int) { Value = salesInvoiceDto.CustomerID }); 
            command.Parameters.Add(new SqlParameter("@WarehouseID", SqlDbType.Int) { Value = salesInvoiceDto.WarehouseID });
            command.Parameters.Add(new SqlParameter("@CreatedByID", SqlDbType.Int) { Value = salesInvoiceDto.CreatedByID });
            command.Parameters.Add(new SqlParameter("@Subtotal", SqlDbType.Decimal) { Value = salesInvoiceDto.Subtotal });
            command.Parameters.Add(new SqlParameter("@DiscountAmount", SqlDbType.Decimal) { Value = salesInvoiceDto.DiscountAmount });
            command.Parameters.Add(new SqlParameter("@TaxAmount", SqlDbType.Decimal) { Value = salesInvoiceDto.TaxAmount });
            command.Parameters.Add(new SqlParameter("@Note", SqlDbType.NVarChar) { Value = (object?)salesInvoiceDto.Note ?? DBNull.Value });

            var records = CreateInvoiceItemRecords(salesInvoiceDto.Items);

            SqlParameter itemsParam = new SqlParameter("@Items", SqlDbType.Structured)
            {
                TypeName = "dbo.InvoiceItemType",
                Value = records
            };
            command.Parameters.Add(itemsParam);

            await connection.OpenAsync(ct); 

            object result = await command.ExecuteScalarAsync(ct);
            if (result != null && result != DBNull.Value)
                newInvoiceId = Convert.ToInt32(result);

            return newInvoiceId;
        }
    }

    public async Task<Invoice?> GetInvoiceByIdAsync(int invoiceId, CancellationToken ct)
    {
        string query = @"SELECT i.InvoiceID, i.InvoiceNumber, i.InvoiceDate, i.StatusID, i.Subtotal, i.DiscountAmount,
                        i.TaxAmount, i.TotalAmount, i.Note, i.WarehouseID, i.CreatedByID, i.CreatedAt AS InvoiceCreatedAt,
                        FROM Invoices i WHERE I.InvoiceID = @InvoiceID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@InvoiceId", SqlDbType.Int) { Value = invoiceId });
            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                if (await reader.ReadAsync(ct))
                {
                    List<InvoiceItem> invoiceItems = await GetInvoiceItemsByInvoiceIdAsync(invoiceId, ct);
                    return MapReaderToInvoice(reader, invoiceItems);
                }
                else
                    return null;
            }
        }
    }

    public async Task<PurchaseInvoice?> GetPurchaseInvoiceByIdAsync(int invoiceId, CancellationToken ct)
    {
        string query = @"SELECT pi.PurchaseInvoiceID, pi.InvoiceID, pi.SupplierID,
                        i.InvoiceNumber, i.InvoiceDate, i.StatusID, i.Subtotal, i.DiscountAmount, i.TaxAmount,
                        i.TotalAmount, i.Note, i.WarehouseID, i.CreatedByID, i.CreatedAt AS InvoiceCreatedAt,
                        s.Name AS SupplierName, s.Phone, s.Email, s.Address, s.TaxNumber, s.IsActive AS IsActiveSupplier, s.CreatedAt AS SupplierCreatedAt,
                        w.Name AS WarehouseName, w.Code, w.[Location], w.IsActive AS IsActiveWarehouse, w.CreatedAt AS WarehouseCreatedAt
                        FROM PurchaseInvoices pi 
                        INNER JOIN Invoices i ON pi.InvoiceID = i.InvoiceID 
                        INNER JOIN Suppliers s ON pi.SupplierID = s.SupplierID
                        INNER JOIN Warehouses w ON i.WarehouseID = w.WarehouseID
                        WHERE i.InvoiceID = @InvoiceID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@InvoiceId", SqlDbType.Int) { Value = invoiceId });
            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                if (await reader.ReadAsync(ct))
                {
                    List<InvoiceItem> invoiceItems = await GetInvoiceItemsByInvoiceIdAsync(invoiceId, ct);
                    return MapReaderToPurchaseInvoice(reader, invoiceItems);
                }
                return null;
            }
        }
    }

    public async Task<SalesInvoice?> GetSalesInvoiceByIdAsync(int invoiceId, CancellationToken ct) {

        string query = @"SELECT si.SalesInvoiceID, si.InvoiceID, si.CustomerID,
                        i.InvoiceNumber, i.InvoiceDate, i.StatusID, i.Subtotal, i.DiscountAmount, i.TaxAmount,
                        i.TotalAmount, i.Note, i.WarehouseID, i.CreatedByID, i.CreatedAt AS InvoiceCreatedAt,
                        c.Name AS CustomerName, c.Email, c.Phone, c.Address, c.IsActive AS IsActiveCustomer, c.CreatedAt AS CustomerCreatedAt, 
                        w.Name AS WarehouseName, w.Code, w.[Location], w.IsActive AS IsActiveWarehouse, w.CreatedAt AS WarehouseCreatedAt
                        FROM SalesInvoices si
                        INNER JOIN Invoices i ON si.InvoiceID = i.InvoiceID 
                        INNER JOIN Customers c ON si.CustomerID = c.CustomerID
                        INNER JOIN Warehouses w ON i.WarehouseID = w.WarehouseID
                        WHERE i.InvoiceID = @InvoiceID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@InvoiceId", SqlDbType.Int) { Value = invoiceId });
            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                if (await reader.ReadAsync(ct))
                {
                    List<InvoiceItem> invoiceItems = await GetInvoiceItemsByInvoiceIdAsync(invoiceId, ct);
                    return MapReaderToSalesInvoice(reader, invoiceItems);
                }
                return null;
            }
        }
    }

    private async Task<List<InvoiceItem>> GetInvoiceItemsByInvoiceIdAsync(int invoiceId, CancellationToken ct)
    {
        List<InvoiceItem> invoiceItems = new List<InvoiceItem>();

        string query = @"SELECT it.InvoiceItemID, it.InvoiceID, it.ProductID, it.Quantity, it.UnitPrice,
                            it.DiscountAmount, it.TaxAmount, it.TotalAmount, 
                            p.Name AS ProductName, p.Sku, p.Barcode, p.Description, p.PurchasePrice,
                            p.SalePrice, p.MinStock, p.IsActive, p.CreatedAt AS ProductCreatedAt, p.UpdatedAt, p.UnitID,
                            p.CategoryID, c.Name AS CategoryName, c.CreatedAt AS CategoryCreatedAt, c.ParentID
                        FROM InvoiceItems it 
                        INNER JOIN Products p ON it.ProductID = p.ProductID
                        LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                        WHERE it.InvoiceID = @InvoiceID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@InvoiceId", SqlDbType.Int) { Value = invoiceId });
            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    InvoiceItem invoiceItem = MapReaderToInvoiceItem(reader);
                    invoiceItems.Add(invoiceItem);
                }
                return invoiceItems;
            }
        }
    }

    public async Task<List<InvoiceSummaryDto>> GetCustomerInvoicesAsync(int customerId, CancellationToken ct, int page = 1, int pageSize = 10)
    {
        string query = @"SELECT i.InvoiceID, i.InvoiceNumber, i.CreatedAt, i.InvoiceType, i.PartyName, i.WarehouseName,
                        i.NetTotal, i.StatusName FROM vw_InvoicesSummary i
                        INNER JOIN SalesInvoices si ON i.InvoiceID = si.InvoiceID
                        WHERE si.CustomerID = @CustomerId
                        ORDER BY i.InvoiceID
                        OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
                        FETCH NEXT @RowsPerPage ROWS ONLY;";

        List<InvoiceSummaryDto> invoiceSummaries = new List<InvoiceSummaryDto>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@CustomerId", SqlDbType.Int) { Value = customerId });
            command.Parameters.Add(new SqlParameter("@PageNumber", SqlDbType.Int) { Value = page });
            command.Parameters.Add(new SqlParameter("@RowsPerPage", SqlDbType.Int) { Value = pageSize });
            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    InvoiceSummaryDto invoiceSummary = MapReaderToInvoiceSummary(reader);
                    invoiceSummaries.Add(invoiceSummary);
                }
                return invoiceSummaries;
            }
        }
    }

    public async Task<int> GetCustomerInvoicesCountAsync(int customerId, CancellationToken ct)
    {
        string query = @"SELECT COUNT(*) FROM vw_InvoicesSummary i
                        INNER JOIN SalesInvoices si ON i.InvoiceID = si.InvoiceID
                        WHERE si.CustomerID = @CustomerId";

        int countNumber = 0;
        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@CustomerId", SqlDbType.Int) { Value = customerId });
            await connection.OpenAsync(ct);

            object result = await command.ExecuteScalarAsync(ct);
            if (result != null && result != DBNull.Value)
                countNumber = Convert.ToInt32(result);

            return countNumber;
        }
    }

    public async Task<List<InvoiceSummaryDto>> GetSupplierInvoicesAsync(int supplierId, CancellationToken ct, int page = 1, int pageSize = 10)
    {
        string query = @"SELECT i.InvoiceID, i.InvoiceNumber, i.CreatedAt, i.InvoiceType, i.PartyName, i.WarehouseName,
                        i.NetTotal, i.StatusName FROM vw_InvoicesSummary i
                        INNER JOIN PurchaseInvoices pi ON i.InvoiceID = pi.InvoiceID
                        WHERE pi.SupplierID = @SupplierID
                        ORDER BY i.InvoiceID
                        OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
                        FETCH NEXT @RowsPerPage ROWS ONLY;";

        List<InvoiceSummaryDto> invoiceSummaries = new List<InvoiceSummaryDto>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@SupplierID", SqlDbType.Int) { Value = supplierId });
            command.Parameters.Add(new SqlParameter("@PageNumber", SqlDbType.Int) { Value = page });
            command.Parameters.Add(new SqlParameter("@RowsPerPage", SqlDbType.Int) { Value = pageSize });
            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    InvoiceSummaryDto invoiceSummary = MapReaderToInvoiceSummary(reader);
                    invoiceSummaries.Add(invoiceSummary);
                }
                return invoiceSummaries;
            }
        }
    }

    public async Task<int> GetSupplierInvoicesCountAsync(int supplierId, CancellationToken ct)
    {
        string query = @"SELECT COUNT(*) FROM vw_InvoicesSummary i
                        INNER JOIN PurchaseInvoices pi ON i.InvoiceID = pi.InvoiceID
                        WHERE pi.SupplierID = @SupplierID";

        int countNumber = 0;
        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@SupplierID", SqlDbType.Int) { Value = supplierId });
            await connection.OpenAsync(ct);

            object result = await command.ExecuteScalarAsync(ct);
            if (result != null && result != DBNull.Value)
                countNumber = Convert.ToInt32(result);

            return countNumber;
        }
    }

    public async Task<List<InvoiceSummaryDto>> GetInvoicesSummaryAsync(CancellationToken ct, int page = 1, int pageSize = 10)
    {
        string query = @"SELECT i.InvoiceID, i.InvoiceNumber, i.CreatedAt, i.InvoiceType, i.PartyName, i.WarehouseName,
                        i.NetTotal, i.StatusName FROM vw_InvoicesSummary i
                        ORDER BY i.InvoiceID
                        OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
                        FETCH NEXT @RowsPerPage ROWS ONLY;";

        List<InvoiceSummaryDto> invoiceSummaries = new List<InvoiceSummaryDto>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@PageNumber", SqlDbType.Int) { Value = page });
            command.Parameters.Add(new SqlParameter("@RowsPerPage", SqlDbType.Int) { Value = pageSize });
            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    InvoiceSummaryDto invoiceSummary = MapReaderToInvoiceSummary(reader);
                    invoiceSummaries.Add(invoiceSummary);
                }
                return invoiceSummaries;
            }
        }
    }

    public async Task<int> GetInvoicesSummaryCountAsync(CancellationToken ct)
    {
        string query = @"SELECT COUNT(*) FROM vw_InvoicesSummary";

        int countNumber = 0;
        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            await connection.OpenAsync(ct);

            object result = await command.ExecuteScalarAsync(ct);
            if (result != null && result != DBNull.Value)
                countNumber = Convert.ToInt32(result);

            return countNumber;
        }
    }

    public async Task<bool> UpdateInvoiceStatusAsync(int invoiceId, int newStatusId, CancellationToken ct)
    {
        string query = @"UPDATE Invoices SET StatusID = @NewStatusId WHERE InvoiceID = @InvoiceId";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@NewStatusId", SqlDbType.Int) { Value = newStatusId });
            command.Parameters.Add(new SqlParameter("@InvoiceId", SqlDbType.Int) { Value = invoiceId });
            await connection.OpenAsync(ct);

            int rowsAffected = await command.ExecuteNonQueryAsync(ct);
            return rowsAffected > 0;
        }
    }

    private InvoiceSummaryDto MapReaderToInvoiceSummary(SqlDataReader reader)
    {
        return new InvoiceSummaryDto
        {
            InvoiceID = reader.GetInt32(reader.GetOrdinal("InvoiceID")),
            InvoiceNumber = reader.GetString(reader.GetOrdinal("InvoiceNumber")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            InvoiceType = reader.GetString(reader.GetOrdinal("InvoiceType")),
            PartyName = reader.GetString(reader.GetOrdinal("PartyName")),
            WarehouseName = reader.GetString(reader.GetOrdinal("WarehouseName")),
            NetTotal = reader.GetDecimal(reader.GetOrdinal("NetTotal")),
            StatusName = reader.GetString(reader.GetOrdinal("StatusName"))
        };
    }

    private Customer MapReaderToCustomer(SqlDataReader reader)
    {
        return new Customer()
        {
            CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
            Name = reader.GetString(reader.GetOrdinal("CustomerName")),
            Phone = reader.GetString(reader.GetOrdinal("Phone")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            Address = reader.GetString(reader.GetOrdinal("Address")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActiveCustomer")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CustomerCreatedAt"))
        };
    }

    private Supplier MapReaderToSupplier(SqlDataReader reader)
    {
        return new Supplier()
        {
            SupplierID = reader.GetInt32(reader.GetOrdinal("SupplierID")),
            Name = reader.GetString(reader.GetOrdinal("SupplierName")),
            Phone = reader.GetString(reader.GetOrdinal("Phone")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            Address = reader.GetString(reader.GetOrdinal("Address")),
            TaxNumber = reader.GetString(reader.GetOrdinal("TaxNumber")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActiveSupplier")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("SupplierCreatedAt"))
        };
    }

    private Warehouse MapReaderToWarehouse(SqlDataReader reader)
    {
        return new Warehouse()
        {
            WarehouseID = reader.GetInt32(reader.GetOrdinal("WarehouseID")),
            Name = reader.GetString(reader.GetOrdinal("WarehouseName")),
            Code = reader.GetString(reader.GetOrdinal("Code")),
            Location = reader.GetString(reader.GetOrdinal("Location")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActiveWarehouse")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("WarehouseCreatedAt")),
        };
    }

    private PurchaseInvoice MapReaderToPurchaseInvoice(SqlDataReader reader, List<InvoiceItem> items)
    {
        return new PurchaseInvoice()
        {
            PurchaseInvoiceID = reader.GetInt32(reader.GetOrdinal("PurchaseInvoiceID")),
            InvoiceID = reader.GetInt32(reader.GetOrdinal("InvoiceID")),
            SupplierID = reader.GetInt32(reader.GetOrdinal("SupplierID")),
            Supplier = MapReaderToSupplier(reader),
            Invoice = MapReaderToInvoice(reader, items)
        };
    }

    private SalesInvoice MapReaderToSalesInvoice(SqlDataReader reader, List<InvoiceItem> items)
    {
        return new SalesInvoice()
        {
            SalesInvoiceID = reader.GetInt32(reader.GetOrdinal("SalesInvoiceID")),
            InvoiceID = reader.GetInt32(reader.GetOrdinal("InvoiceID")),
            CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
            Customer = MapReaderToCustomer(reader),
            Invoice = MapReaderToInvoice(reader, items)
        };
    }

    private Invoice MapReaderToInvoice(SqlDataReader reader, List<InvoiceItem> items)
    {
        return new Invoice
        {
            InvoiceID = reader.GetInt32(reader.GetOrdinal("InvoiceID")),
            InvoiceNumber = reader.GetString(reader.GetOrdinal("InvoiceNumber")),
            InvoiceDate = reader.GetDateTime(reader.GetOrdinal("InvoiceDate")),
            StatusID = reader.GetByte(reader.GetOrdinal("StatusID")),
            Subtotal = reader.GetDecimal(reader.GetOrdinal("Subtotal")),
            DiscountAmount = reader.GetDecimal(reader.GetOrdinal("DiscountAmount")),
            TaxAmount = reader.GetDecimal(reader.GetOrdinal("TaxAmount")),
            TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
            Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? null : reader.GetString(reader.GetOrdinal("Note")),
            WarehouseID = reader.GetInt32(reader.GetOrdinal("WarehouseID")),
            CreatedByID = reader.GetInt32(reader.GetOrdinal("CreatedByID")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("InvoiceCreatedAt")),
            InvoiceItems = items,
            Warehouse = MapReaderToWarehouse(reader)
        };
    }

    private InvoiceItem MapReaderToInvoiceItem(SqlDataReader reader)
    {
        return new InvoiceItem
        {
            InvoiceItemID = reader.GetInt32(reader.GetOrdinal("InvoiceItemID")),
            InvoiceID = reader.GetInt32(reader.GetOrdinal("InvoiceID")),
            ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
            Quantity = reader.GetDecimal(reader.GetOrdinal("Quantity")),
            UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
            DiscountAmount = reader.GetDecimal(reader.GetOrdinal("DiscountAmount")),
            TaxAmount = reader.GetDecimal(reader.GetOrdinal("TaxAmount")),
            TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
            Product = MapReaderToProduct(reader)
        };
    }

    private Product MapReaderToProduct(SqlDataReader reader)
    {
        return new Product()
        {
            ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
            Name = reader.GetString(reader.GetOrdinal("ProductName")),
            Sku = reader.GetString(reader.GetOrdinal("Sku")),
            Barcode = reader.GetString(reader.GetOrdinal("Barcode")),
            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ?
                null : reader.GetString(reader.GetOrdinal("Description")),
            PurchasePrice = reader.GetDecimal(reader.GetOrdinal("PurchasePrice")),
            SalePrice = reader.GetDecimal(reader.GetOrdinal("SalePrice")),
            MinStock = reader.GetInt32(reader.GetOrdinal("MinStock")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("ProductCreatedAt")),
            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ?
                null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
            UnitID = reader.GetInt32(reader.GetOrdinal("UnitID")),
            CategoryID = reader.IsDBNull(reader.GetOrdinal("CategoryID")) ?
                 null : reader.GetInt32(reader.GetOrdinal("CategoryID")),
            Category = reader.IsDBNull(reader.GetOrdinal("CategoryID")) ? null : new Category()
            {
                CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                Name = reader.GetString(reader.GetOrdinal("CategoryName")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CategoryCreatedAt")),
            }
        };
    }

    private IEnumerable<SqlDataRecord> CreateInvoiceItemRecords(List<CreateInvoiceItemDto> items)
    {
        SqlMetaData[] schema =
        [
            new SqlMetaData("ProductID", SqlDbType.Int),
            new SqlMetaData("Quantity", SqlDbType.Decimal, 18, 2),
            new SqlMetaData("UnitPrice", SqlDbType.Decimal, 18, 2),
            new SqlMetaData("DiscountAmount", SqlDbType.Decimal, 18, 2),
            new SqlMetaData("TaxAmount", SqlDbType.Decimal, 18, 2)
        ];

        foreach (var item in items)
        {
            SqlDataRecord record = new SqlDataRecord(schema);
            record.SetInt32(0, item.ProductID);
            record.SetDecimal(1, item.Quantity);
            record.SetDecimal(2, item.UnitPrice);
            record.SetDecimal(3, item.DiscountAmount);
            record.SetDecimal(4, item.TaxAmount);
            yield return record;
        }
    }
}