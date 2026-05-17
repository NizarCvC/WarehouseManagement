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

            connection.Open();

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
            command.Parameters.Add(new SqlParameter("@SupplierID", SqlDbType.Int) { Value = salesInvoiceDto.CustomerID });
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

            connection.Open();

            object result = await command.ExecuteScalarAsync(ct);
            if (result != null && result != DBNull.Value)
                newInvoiceId = Convert.ToInt32(result);

            return newInvoiceId;
        }
    }

    // TODO: Need to implement this method to retrieve full invoice details including items, warehouse, and user info
    public async Task<Invoice?> GetInvoiceByIdAsync(int invoiceId, CancellationToken ct)
    {
        throw new NotImplementedException();
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
        string query = @"UPDATE Invoices
                        SET StatusID = @NewStatusId
                        WHERE InvoiceID = @InvoiceId";

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
            InvoiceID = reader.GetInt32("InvoiceID"),
            InvoiceNumber = reader.GetString("InvoiceNumber"),
            CreatedAt = reader.GetDateTime("CreatedAt"),
            InvoiceType = reader.GetString("InvoiceType"),
            PartyName = reader.GetString("PartyName"),
            WarehouseName = reader.GetString("WarehouseName"),
            NetTotal = reader.GetDecimal("NetTotal"),
            StatusName = reader.GetString("StatusName")
        };
    }

    // TODO: This method can be expanded to include more fields as needed
    private Invoice MapReaderToInvoice(SqlDataReader reader)
    {
        return new Invoice
        {
            InvoiceID = reader.GetInt32("InvoiceID"),
            InvoiceNumber = reader.GetString("InvoiceNumber"),
            InvoiceDate = reader.GetDateTime("InvoiceDate"),
            StatusID = reader.GetByte("StatusID"),
            Subtotal = reader.GetDecimal("Subtotal"),
            DiscountAmount = reader.GetDecimal("DiscountAmount"),
            TaxAmount = reader.GetDecimal("TaxAmount"),
            TotalAmount = reader.GetDecimal("TotalAmount"),
            Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? null : reader.GetString("Note"),
            WarehouseID = reader.GetInt32("WarehouseID"),
            CreatedByID = reader.GetInt32("CreatedByID"),
            CreatedAt = reader.GetDateTime("CreatedAt"),
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
