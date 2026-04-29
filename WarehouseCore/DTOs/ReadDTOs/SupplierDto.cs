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
}