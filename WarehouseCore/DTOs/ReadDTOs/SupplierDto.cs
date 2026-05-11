using WarehouseCore.Entities;

namespace WarehouseCore.DTOs.ReadDTOs;

public class SupplierDto
{
    public int SupplierID { get; set; }
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public required string Email { get; set; }
    public required string Address { get; set; }
    public required string TaxNumber { get; set; }
    public bool IsActive { get; set; }

    public static SupplierDto FromEntity(Supplier supplier)
    {
        return new SupplierDto()
        {
            SupplierID = supplier.SupplierID,
            Name = supplier.Name,
            Phone = supplier.Phone,
            Email = supplier.Email,
            Address = supplier.Address,
            TaxNumber = supplier.TaxNumber,
            IsActive = supplier.IsActive
        };
    }

    public static List<SupplierDto> FromEntities(List<Supplier> suppliers)
    {
        return suppliers.Select(FromEntity).ToList();
    }
}