using WarehouseCore.Entities;

namespace WarehouseCore.DTOs.ReadDTOs;

public class WarehouseDto
{
    public int WarehouseID { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required string Location { get; set; }
    public bool IsActive { get; set; }

    public static WarehouseDto FromEntity(Warehouse warehouse)
    {
        return new WarehouseDto()
        {
            WarehouseID = warehouse.WarehouseID,
            Name = warehouse.Name,
            Code = warehouse.Code,
            Location = warehouse.Location,
            IsActive = warehouse.IsActive
        };
    }

    public static List<WarehouseDto> FromEntities(List<Warehouse> warehouses)
    {
        return warehouses.Select(FromEntity).ToList();
    }
}
