using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;

namespace WarehouseDataAccess.Interfaces;

public interface IInvoiceRepository
{
    Task<int> CreateSalesInvoiceAsync(CreateSalesInvoiceDto salesInvoiceDto, CancellationToken ct);
    Task<int> CreatePurchaseInvoiceAsync(CreatePurchaseInvoiceDto purchaseInvoiceDto, CancellationToken ct);
    Task<bool> UpdateInvoiceStatusAsync(int invoiceId, int newStatusId, CancellationToken ct);
    Task<List<InvoiceSummaryDto>> GetInvoicesSummaryAsync(CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetInvoicesSummaryCountAsync(CancellationToken ct);
    Task<List<InvoiceSummaryDto>> GetCustomerInvoicesAsync(int customerId, CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetCustomerInvoicesCountAsync(int customerId, CancellationToken ct);
    Task<List<InvoiceSummaryDto>> GetSupplierInvoicesAsync(int supplierId, CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetSupplierInvoicesCountAsync(int supplierId, CancellationToken ct);
    Task<Invoice?> GetInvoiceByIdAsync(int invoiceId, CancellationToken ct);
}