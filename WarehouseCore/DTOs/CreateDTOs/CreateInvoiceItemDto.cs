namespace WarehouseCore.DTOs.CreateDTOs;

public class CreateInvoiceItemDto
{
    public int ProductID { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
}
