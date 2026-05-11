using WarehouseCore.enums;

namespace WarehouseCore.Entities;

public class Product
{
    public int ProductID { get; init; }
    public required string Name { get; init; }
    public required string Sku { get; init; }
    public required string Barcode { get; init; }
    public string? Description { get; init; }
    public decimal PurchasePrice { get; init; }
    public decimal SalePrice { get; init; }
    public int MinStock { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public enUnit Unit { get; init; }
    public int? CategoryID { get; init; }
    public Category? Category { get; init; }
}
