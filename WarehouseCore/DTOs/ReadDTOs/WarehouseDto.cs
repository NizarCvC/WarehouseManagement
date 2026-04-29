namespace WarehouseCore.DTOs.ReadDTOs;

public class WarehouseDto
{
    public int WarehouseID { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required string Location { get; set; }
    public bool IsActive { get; set; }
}
