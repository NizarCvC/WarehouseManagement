namespace WarehouseCore.DTOs.ReadDTOs;

public class ProductDto
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
    public string? CategoryName { get; set; } 
    public required string UnitName { get; set; } 
}
