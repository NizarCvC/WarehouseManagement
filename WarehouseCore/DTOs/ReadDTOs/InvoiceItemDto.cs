namespace WarehouseCore.DTOs.ReadDTOs;

public class InvoiceItemDto
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public required string Sku { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal => (Quantity * UnitPrice) - DiscountAmount + TaxAmount;
}