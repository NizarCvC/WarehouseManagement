namespace WarehouseCore.Entities;

public class Warehouse
{
    public int WarehouseID { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required string Location { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
