using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;

namespace WarehouseDataAccess.Interfaces;

public interface IInvoiceRepository
{
    Task<bool> ApproveSalesInvoiceAsync(CreateSalesInvoiceDto salesInvoiceDto, CancellationToken ct);
    Task<bool> ApprovePurchaseInvoiceAsync(CreatePurchaseInvoiceDto purchaseInvoiceDto, CancellationToken ct);
    Task<List<InvoiceSummaryDto>> GetInvoicesSummaryAsync(CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetInvoicesSummaryCountAsync(CancellationToken ct);
    Task<List<InvoiceSummaryDto>> GetInvoicesSummaryAsync(int invoiceId, CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetInvoicesSummaryCountAsync(int invoiceId, CancellationToken ct);
    Task<Invoice?> GetInvoiceByIdAsync(int invoiceId, CancellationToken ct);
}