namespace WarehouseCore.Entities;

public class Category
{
    public required int CategoryID { get; init; }
    public required string Name { get; init; }
    public required DateTime CreatedAt { get; init; }
    public int? ParentID { get; init; }
    public Category? Parent { get; init; }
}
