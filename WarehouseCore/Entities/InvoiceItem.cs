namespace WarehouseCore.Entities;

public class InvoiceItem
{
    public int InvoiceItemID { get; set; }
    public int InvoiceID { get; set; }
    public int ProductID { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
}
