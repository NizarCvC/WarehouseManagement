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

public class InvoiceService(IInvoiceRepository invoiceRepository, IWarehouseService warehouseService,
    ICustomerService customerService, ISupplierService supplierService,
    ILogger<InvoiceService> logger) : IInvoiceService
{
    public async Task<int> CreatePurchaseInvoiceAsync(CreatePurchaseInvoiceDto purchaseInvoiceDto, CancellationToken ct)
    {
        if (!CrossFinancialValidation(purchaseInvoiceDto))
        {
            throw new BadRequestException("The calculated subtotal, tax amount, or discount amount does not match the provided values.");
        }

        WarehouseDto warehouseDto = await warehouseService.GetWarehouseByIdAsync(purchaseInvoiceDto.WarehouseID, ct);

        if (!warehouseDto.IsActive)
        {
            throw new BadRequestException($"The warehouse ID: {purchaseInvoiceDto.WarehouseID} is not active.");
        }

        SupplierDto supplierDto = await supplierService.GetSupplierByIdAsync(purchaseInvoiceDto.SupplierID, ct);

        if (!supplierDto.IsActive)
        {
            throw new BadRequestException($"The supplier ID: {purchaseInvoiceDto.SupplierID} is not active.");
        }

        try
        {
            int newInvoiceId = await invoiceRepository.CreatePurchaseInvoiceAsync(purchaseInvoiceDto, ct);
            logger.LogInformation("The purchase invoice created for supplier ID: {SupplierId} and warehouse ID: {WarehouseId}", purchaseInvoiceDto.SupplierID, purchaseInvoiceDto.WarehouseID);
            return newInvoiceId;
        }
        catch (SqlException ex) when (ex.Number == 50001)
        {
            throw new BadRequestException(ex.Message);
        }
    }

    public async Task<int> CreateSalesInvoiceAsync(CreateSalesInvoiceDto salesInvoiceDto, CancellationToken ct)
    {
        if (!CrossFinancialValidation(salesInvoiceDto))
        {
            throw new BadRequestException("The calculated subtotal, tax amount, or discount amount does not match the provided values.");
        }

        WarehouseDto warehouseDto = await warehouseService.GetWarehouseByIdAsync(salesInvoiceDto.WarehouseID, ct);

        if (!warehouseDto.IsActive)
        {
            throw new BadRequestException($"The warehouse ID: {salesInvoiceDto.WarehouseID} is not active.");
        }

        CustomerDto customerDto = await customerService.GetCustomerByIdAsync(salesInvoiceDto.CustomerID, ct);

        if (!customerDto.IsActive)
        {
            throw new BadRequestException($"The customer ID: {salesInvoiceDto.CustomerID} is not active.");
        }

        try
        {
            int newInvoiceId = await invoiceRepository.CreateSalesInvoiceAsync(salesInvoiceDto, ct);
            logger.LogInformation("The sales invoice created for customer ID: {CustomerId} and warehouse ID: {WarehouseId}", salesInvoiceDto.CustomerID, salesInvoiceDto.WarehouseID);
            return newInvoiceId;
        }
        catch (SqlException ex) when (ex.Number == 50001)
        {
            throw new BadRequestException(ex.Message);
        }
        catch (SqlException ex) when (ex.Number == 50002)
        {
            throw new BadRequestException(ex.Message);
        }
    }

    public async Task<List<InvoiceSummaryDto>> GetCustomerInvoicesAsync(int customerId, CancellationToken ct, int page = 1, int pageSize = 10)
    {
        if (!await customerService.IsCustomerExistsByIdAsync(customerId, ct))
        {
            throw new NotFoundException($"The customer ID: {customerId} not exists.");
        }

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
        if (!await supplierService.IsSupplierExistsByIdAsync(supplierId, ct))
            throw new NotFoundException($"The supplier ID: {supplierId} not exists.");

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

        if (invoice.InvoiceStatus == enInvoiceStatus.Paid || invoice.InvoiceStatus == enInvoiceStatus.Cancelled)
            throw new BadRequestException($"The invoice ID: {invoiceId} is already paid or cancelled and cannot be updated.");

        if (!Enum.IsDefined(typeof(enInvoiceStatus), newStatusId))
            throw new NotFoundException($"The invoice status ID: {newStatusId} not exists.");

        await invoiceRepository.UpdateInvoiceStatusAsync(invoiceId, newStatusId, ct);
        logger.LogInformation("The invoice ID: {InvoiceId} status updated to {NewStatusId}", invoiceId, newStatusId);
    }

    private bool CrossFinancialValidation(CreateInvoiceDto createInvoiceDto) 
    {
        decimal calcSubtotal = createInvoiceDto.Items.Sum(e => e.UnitPrice * e.Quantity);
        decimal calcTaxAmount = createInvoiceDto.Items.Sum(e => e.TaxAmount);
        decimal calcDiscountAmount = createInvoiceDto.Items.Sum(e => e.DiscountAmount);

        return calcSubtotal == createInvoiceDto.Subtotal 
            && calcTaxAmount == createInvoiceDto.TaxAmount 
            && calcDiscountAmount == createInvoiceDto.DiscountAmount;
    }
}
