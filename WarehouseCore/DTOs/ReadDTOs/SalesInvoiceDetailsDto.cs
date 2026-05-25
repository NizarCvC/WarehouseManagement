using WarehouseCore.Entities;

namespace WarehouseCore.DTOs.ReadDTOs;

public class SalesInvoiceDetailsDto
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
    public int CustomerId { get; set; }
    public required string CustomerName { get; set; }
    public required string WarehouseName { get; set; }
    public ICollection<InvoiceItemDto> Items { get; set; } = [];

    public static SalesInvoiceDetailsDto FromEntity(SalesInvoice invoice)
    {
        return new SalesInvoiceDetailsDto
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
            CustomerId = invoice.CustomerID,
            CustomerName = invoice.Customer.Name,
            WarehouseName = invoice.Invoice.Warehouse.Name,
            Items = invoice.Invoice.InvoiceItems.Select(InvoiceItemDto.FromEntity).ToList()
        };
    }

    public static List<SalesInvoiceDetailsDto> FromEntities(List<SalesInvoice> invoices)
    {
        return invoices.Select(FromEntity).ToList();
    }
}
