using WarehouseCore.CustomTypes;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;

namespace WarehouseServices.Interfaces;

public interface IInvoiceService
{
    Task<bool> ApproveSalesInvoiceAsync(Invoice invoice, int customerId, List<InvoiceItemType> items);
    Task<bool> ApprovePurchaseInvoiceAsync(Invoice invoice, int supplierId, List<InvoiceItemType> items);
    Task<List<InvoiceSummaryDto>> GetInvoicesSummaryAsync(int page = 1, int pageSize = 10);
    Task<int> GetInvoicesSummaryCountAsync();
    Task<List<InvoiceSummaryDto>> GetInvoicesSummaryAsync(int invoiceId, int page = 1, int pageSize = 10);
    Task<int> GetInvoicesSummaryCountAsync(int invoiceId);
    Task<Invoice?> GetInvoiceByIdAsync(int id);
}
