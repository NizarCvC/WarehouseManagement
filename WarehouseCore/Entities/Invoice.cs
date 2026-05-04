namespace WarehouseCore.Entities;

public class Invoice
{
    public int InvoiceID { get; init; }
    public required string InvoiceNumber { get; init; }
    public DateTime InvoiceDate { get; init; }
    public byte StatusID { get; init; }
    public decimal Subtotal { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public string? Note { get; init; }
    public int WarehouseID { get; init; }
    public Warehouse? Warehouse { get; init; }
    public int CreatedByID { get; init; }
    public User? CreatedBy { get; init; }
    public DateTime CreatedAt { get; init; }
    public List<InvoiceItem>? InvoiceItems;
}
