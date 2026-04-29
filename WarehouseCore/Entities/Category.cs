namespace WarehouseCore.Entities;

public class Category
{
    public int CategoryID { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? ParentID { get; set; }
}
