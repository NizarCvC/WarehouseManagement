namespace WarehouseCore.Entities;

public class Customer
{
    public int CustomerID { get; set; }
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public required string Email { get; set; }
    public required string Address { get; set; }
    public bool IsActive { get; set; } 
    public DateTime CreatedAt { get; set; }
}
