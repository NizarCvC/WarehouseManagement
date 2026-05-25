using WarehouseCore.enums;

namespace WarehouseCore.Entities;

public class Invoice
{
    public required int InvoiceID { get; init; }
    public required string InvoiceNumber { get; init; }
    public required DateTime InvoiceDate { get; init; }
    public required byte StatusID { get; init; }
    public enInvoiceStatus invoiceStatus { get => (enInvoiceStatus)StatusID; }
    public required decimal Subtotal { get; init; }
    public required decimal DiscountAmount { get; init; }
    public required decimal TaxAmount { get; init; }
    public required decimal TotalAmount { get; init; }
    public string? Note { get; init; }
    public required int WarehouseID { get; init; }
    public Warehouse Warehouse { get; init; } = null!;
    public required int CreatedByID { get; init; }
    public User CreatedBy { get; init; }  = null!;
    public required DateTime CreatedAt { get; init; }
    public ICollection<InvoiceItem> InvoiceItems { get; init; } = [];
}
