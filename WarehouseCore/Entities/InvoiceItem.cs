namespace WarehouseCore.Entities;

public class InvoiceItem
{
    public required int InvoiceItemID { get; init; }
    public required int InvoiceID { get; init; }
    public Invoice Invoice { get; init; } = null!;
    public required int ProductID { get; init; }
    public Product Product { get; init; } = null!;
    public required decimal Quantity { get; init; }
    public required decimal UnitPrice { get; init; }
    public required decimal DiscountAmount { get; init; }
    public required decimal TaxAmount { get; init; }
    public required decimal TotalAmount { get; init; }
}
