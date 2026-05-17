using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;

namespace WarehouseServices.Interfaces;

public interface IInvoiceService
{
    Task<int> CreateSalesInvoiceAsync(CreateSalesInvoiceDto salesInvoiceDto, CancellationToken ct);
    Task<int> CreatePurchaseInvoiceAsync(CreatePurchaseInvoiceDto purchaseInvoiceDto, CancellationToken ct);
    Task UpdateInvoiceStatusAsync(int invoiceId, int newStatusId, CancellationToken ct);
    Task<List<InvoiceSummaryDto>> GetInvoicesSummaryAsync(CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetInvoicesSummaryCountAsync(CancellationToken ct);
    Task<List<InvoiceSummaryDto>> GetCustomerInvoicesAsync(int customerId, CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetCustomerInvoicesCountAsync(int customerId, CancellationToken ct);
    Task<List<InvoiceSummaryDto>> GetSupplierInvoicesAsync(int supplierId, CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetSupplierInvoicesCountAsync(int supplierId, CancellationToken ct);
    Task<SalesInvoiceDetailsDto> GetSalesInvoiceByIdAsync(int invoiceId, CancellationToken ct);
    Task<PurchaseInvoiceDetailsDto> GetPurchaseInvoiceByIdAsync(int invoiceId, CancellationToken ct);
}