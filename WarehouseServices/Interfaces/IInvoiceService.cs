using WarehouseCore.CustomTypes;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;

namespace WarehouseServices.Interfaces;

public interface IInvoiceService
{
    Task<bool> ApproveSalesInvoiceAsync(Invoice invoice, int customerId, List<InvoiceItemType> items, CancellationToken ct);
    Task<bool> ApprovePurchaseInvoiceAsync(Invoice invoice, int supplierId, List<InvoiceItemType> items, CancellationToken ct);
    Task<List<InvoiceSummaryDto>> GetInvoicesSummaryAsync(CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetInvoicesSummaryCountAsync(CancellationToken ct);
    Task<List<InvoiceSummaryDto>> GetInvoicesSummaryAsync(int invoiceId, CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetInvoicesSummaryCountAsync(int invoiceId, CancellationToken ct);
    Task<Invoice?> GetInvoiceByIdAsync(int invoiceId, CancellationToken ct);
}
