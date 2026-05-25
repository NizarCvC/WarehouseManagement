namespace WarehouseCore.Entities;

public class Warehouse
{
    public required int WarehouseID { get; init; }
    public required string Name { get; init; }
    public required string Code { get; init; }
    public required string Location { get; init; }
    public required bool IsActive { get; init; }
    public required DateTime CreatedAt { get; init; }
}
