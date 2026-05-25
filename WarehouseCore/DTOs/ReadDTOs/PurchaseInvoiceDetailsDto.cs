using WarehouseCore.Entities;

namespace WarehouseCore.DTOs.ReadDTOs;

public class PurchaseInvoiceDetailsDto
{
    public int InvoiceId { get; set; }
    public required string InvoiceNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string Status { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Note { get; set; }
    public int SupplierId { get; set; }
    public required string SupplierName { get; set; }
    public required string WarehouseName { get; set; }
    public ICollection<InvoiceItemDto> Items { get; set; } = [];

    public static PurchaseInvoiceDetailsDto FromEntity(PurchaseInvoice invoice)
    {
        return new PurchaseInvoiceDetailsDto
        {
            InvoiceId = invoice.InvoiceID,
            InvoiceNumber = invoice.Invoice.InvoiceNumber,
            CreatedAt = invoice.Invoice.CreatedAt,
            Status = invoice.Invoice.invoiceStatus.ToString(),
            Subtotal = invoice.Invoice.Subtotal,
            DiscountAmount = invoice.Invoice.DiscountAmount,
            TaxAmount = invoice.Invoice.TaxAmount,
            TotalAmount = invoice.Invoice.TotalAmount,
            Note = invoice.Invoice.Note,
            SupplierId = invoice.SupplierID,
            SupplierName = invoice.Supplier.Name,
            WarehouseName = invoice.Invoice.Warehouse.Name,
            Items = invoice.Invoice.InvoiceItems.Select(InvoiceItemDto.FromEntity).ToList()
        };
    }

    public static List<PurchaseInvoiceDetailsDto> FromEntities(List<PurchaseInvoice> invoices)
    {
        return invoices.Select(FromEntity).ToList();
    }
}