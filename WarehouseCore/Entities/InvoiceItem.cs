namespace WarehouseCore.Entities;

public class InvoiceItem
{
    public int InvoiceItemID { get; init; }
    public int InvoiceID { get; init; }
    public Invoice? Invoice { get; init; }
    public int ProductID { get; init; }
    public Product Product { get; init; } = null!;
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal TotalAmount { get; init; }
}
