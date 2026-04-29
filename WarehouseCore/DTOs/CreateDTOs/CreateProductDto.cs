namespace WarehouseCore.DTOs.CreateDTOs;

public class CreateProductDto
{
    public required string Name { get; set; }
    public required string Sku { get; set; }
    public required string Barcode { get; set; }
    public string? Description { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public int MinStock { get; set; }
    public int? CategoryID { get; set; }
    public int UnitID { get; set; }
}
