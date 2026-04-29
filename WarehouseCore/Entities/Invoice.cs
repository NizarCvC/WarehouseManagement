namespace WarehouseCore.Entities;

public class Invoice
{
    public int InvoiceID { get; set; }
    public required string InvoiceNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public byte StatusID { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Note { get; set; }
    public int WarehouseID { get; set; }
    public int CreatedByID { get; set; }
    public DateTime CreatedAt { get; set; }
}
