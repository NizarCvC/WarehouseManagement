namespace WarehouseCore.Entities;

public class Customer
{
    public int CustomerID { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Phone { get; init; }
    public required string Address { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}
