namespace WarehouseCore.Entities;

public class Category
{
    public int CategoryID { get; init; }
    public required string Name { get; init; }
    public DateTime CreatedAt { get; init; }
    public int? ParentID { get; init; }
    public Category? Parent { get; init; }
}
