using WarehouseCore.Entities;

namespace WarehouseCore.DTOs.ReadDTOs;

public class InvoiceItemDto
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public required string Sku { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal => (Quantity * UnitPrice) - DiscountAmount + TaxAmount;

    public static InvoiceItemDto FromEntity(InvoiceItem invoiceItem)
    {
        return new InvoiceItemDto()
        {
            ProductId = invoiceItem.ProductID,
            ProductName = invoiceItem.Product.Name,
            Sku = invoiceItem.Product.Sku,
            Quantity = invoiceItem.Quantity,
            UnitPrice = invoiceItem.UnitPrice,
            DiscountAmount = invoiceItem.DiscountAmount,
            TaxAmount = invoiceItem.TaxAmount
        };
    }

    public static List<InvoiceItemDto> FromEntities(List<InvoiceItem> invoiceItems)
    {
        return invoiceItems.Select(FromEntity).ToList();
    }
}