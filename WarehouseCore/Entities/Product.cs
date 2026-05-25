using WarehouseCore.enums;

namespace WarehouseCore.Entities;

public class Product
{
    public required int ProductID { get; init; }
    public required string Name { get; init; }
    public required string Sku { get; init; }
    public required string Barcode { get; init; }
    public string? Description { get; init; }
    public required decimal PurchasePrice { get; init; }
    public required decimal SalePrice { get; init; }
    public required int MinStock { get; init; }
    public required bool IsActive { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public required int UnitID { get; init; }
    public enUnit Unit { get => (enUnit)UnitID; } 
    public int? CategoryID { get; init; }
    public Category? Category { get; init; }
}
