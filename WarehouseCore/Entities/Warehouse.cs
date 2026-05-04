namespace WarehouseCore.Entities;

public class Warehouse
{
    public int WarehouseID { get; init; }
    public required string Name { get; init; }
    public required string Code { get; init; }
    public required string Location { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}
