namespace WarehouseCore.DTOs.CreateDTOs;

public class CreateSupplierDto
{
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public required string Email { get; set; }
    public required string Address { get; set; }
    public required string TaxNumber { get; set; }
}
