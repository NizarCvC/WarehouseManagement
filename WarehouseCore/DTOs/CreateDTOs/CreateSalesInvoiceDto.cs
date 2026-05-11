namespace WarehouseCore.DTOs.CreateDTOs;

public class CreateSalesInvoiceDto
{
    public required string InvoiceNumber { get; set; }
    public required int CustomerID { get; set; }
    public required int WarehouseID { get; set; }
    public required int CreatedByID { get; set; }
    public required decimal Subtotal { get; set; }
    public required decimal DiscountAmount { get; set; }
    public required decimal TaxAmount { get; set; }
    public required string? Note { get; set; }

    public required List<CreateInvoiceItemDto> Items { get; set; }
}
