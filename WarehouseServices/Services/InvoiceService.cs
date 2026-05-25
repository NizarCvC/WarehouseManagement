using Microsoft.Extensions.Logging;
using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;
using WarehouseCore.enums;
using WarehouseDataAccess.Interfaces;
using WarehouseServices.Exceptions;
using WarehouseServices.Interfaces;

namespace WarehouseServices.Services;

public class InvoiceService(IInvoiceRepository invoiceRepository, 
    ILogger<InvoiceService> logger) : IInvoiceService
{
    public async Task<int> CreatePurchaseInvoiceAsync(CreatePurchaseInvoiceDto purchaseInvoiceDto, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<int> CreateSalesInvoiceAsync(CreateSalesInvoiceDto salesInvoiceDto, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<List<InvoiceSummaryDto>> GetCustomerInvoicesAsync(int customerId, CancellationToken ct, int page = 1, int pageSize = 10)
    {
        List<InvoiceSummaryDto> invoiceSummaries = await invoiceRepository.GetCustomerInvoicesAsync(customerId, ct, page, pageSize);
        logger.LogInformation("The customer invoices retrieved for customer ID: {CustomerId} in page {Page} with page size {PageSize}", customerId, page, pageSize);
        return invoiceSummaries;
    }

    public async Task<int> GetCustomerInvoicesCountAsync(int customerId, CancellationToken ct)
    {
        int count = await invoiceRepository.GetCustomerInvoicesCountAsync(customerId, ct);
        logger.LogInformation("The customer invoices count retrieved for customer ID: {CustomerId}", customerId);
        return count;
    }

    public async Task<List<InvoiceSummaryDto>> GetInvoicesSummaryAsync(CancellationToken ct, int page = 1, int pageSize = 10)
    {
        List<InvoiceSummaryDto> invoiceSummaries = await invoiceRepository.GetInvoicesSummaryAsync(ct, page, pageSize);
        logger.LogInformation("The invoice summaries retrieved in page {Page} with page size {PageSize}", page, pageSize);
        return invoiceSummaries;
    }

    public async Task<int> GetInvoicesSummaryCountAsync(CancellationToken ct)
    {
        int count = await invoiceRepository.GetInvoicesSummaryCountAsync(ct);
        logger.LogInformation("The invoice summaries count retrieved");
        return count;
    }

    public async Task<PurchaseInvoiceDetailsDto> GetPurchaseInvoiceByIdAsync(int invoiceId, CancellationToken ct)
    {
        PurchaseInvoice? purchaseInvoice = await invoiceRepository.GetPurchaseInvoiceByIdAsync(invoiceId, ct);

        if (purchaseInvoice is null)
            throw new NotFoundException($"The purchase invoice ID: {invoiceId} not exists.");

        logger.LogInformation("The purchase invoice id '{InvoiceId}' is retrieved", invoiceId);
        return PurchaseInvoiceDetailsDto.FromEntity(purchaseInvoice);
    }

    public async Task<SalesInvoiceDetailsDto> GetSalesInvoiceByIdAsync(int invoiceId, CancellationToken ct)
    {
        SalesInvoice? salesInvoice = await invoiceRepository.GetSalesInvoiceByIdAsync(invoiceId, ct);

        if (salesInvoice is null)
            throw new NotFoundException($"The sales invoice ID: {invoiceId} not exists.");

        logger.LogInformation("The sales invoice id '{InvoiceId}' is retrieved", invoiceId);
        return SalesInvoiceDetailsDto.FromEntity(salesInvoice);
    }

    public async Task<List<InvoiceSummaryDto>> GetSupplierInvoicesAsync(int supplierId, CancellationToken ct, int page = 1, int pageSize = 10)
    {
        List<InvoiceSummaryDto> invoiceSummaries = await invoiceRepository.GetSupplierInvoicesAsync(supplierId, ct, page, pageSize);
        logger.LogInformation("The supplier invoices retrieved for supplier ID: {SupplierId} in page {Page} with page size {PageSize}", supplierId, page, pageSize);
        return invoiceSummaries;
    }

    public async Task<int> GetSupplierInvoicesCountAsync(int supplierId, CancellationToken ct)
    {
        int count = await invoiceRepository.GetSupplierInvoicesCountAsync(supplierId, ct);
        logger.LogInformation("The supplier invoices count retrieved for supplier ID: {SupplierId}", supplierId);
        return count;
    }

    public async Task UpdateInvoiceStatusAsync(int invoiceId, int newStatusId, CancellationToken ct)
    {
        Invoice? invoice = await invoiceRepository.GetInvoiceByIdAsync(invoiceId, ct);

        if (invoice is null)
            throw new NotFoundException($"The invoice ID: {invoiceId} not exists.");

        if (!Enum.IsDefined(typeof(enInvoiceStatus), newStatusId))
            throw new NotFoundException($"The invoice status ID: {newStatusId} not exists.");

        await invoiceRepository.UpdateInvoiceStatusAsync(invoiceId, newStatusId, ct);
    }
}
