namespace WarehouseCore.DTOs.ReadDTOs;

public class PurchaseInvoiceDetailsDto
{
    public int InvoiceId { get; set; }
    public required string InvoiceNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string Status { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Note { get; set; }
    public int SupplierId { get; set; }
    public required string SupplierName { get; set; }
    public required string WarehouseName { get; set; }
    public ICollection<InvoiceItemDto> Items { get; set; } = [];
}