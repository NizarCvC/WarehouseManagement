namespace WarehouseCore.DTOs.CreateDTOs;

public class CreateCustomerDto
{
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public required string Email { get; set; }
    public required string Address { get; set; }
}
