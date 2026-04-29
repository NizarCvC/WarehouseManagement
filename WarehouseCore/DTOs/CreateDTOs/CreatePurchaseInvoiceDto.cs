namespace WarehouseCore.DTOs.CreateDTOs;

public class CreatePurchaseInvoiceDto
{
    public required string InvoiceNumber { get; set; }
    public int SupplierID { get; set; }
    public int WarehouseID { get; set; }
    public int CreatedByID { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public string? Note { get; set; }

    public required List<CreateInvoiceItemDto> Items { get; set; }
}
