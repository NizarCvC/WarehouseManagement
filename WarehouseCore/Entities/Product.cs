namespace WarehouseCore.Entities;

public class Product
{
    public int ProductID { get; set; }
    public required string Name { get; set; }
    public required string Sku { get; set; }
    public required string Barcode { get; set; }
    public string? Description { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public int MinStock { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? CategoryID { get; set; }
    public int UnitID { get; set; }
}
