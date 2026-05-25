namespace WarehouseCore.Entities;

public class Supplier
{
    public required int SupplierID { get; init; }
    public required string Name { get; init; }
    public required string Phone { get; init; }
    public required string Email { get; init; }
    public required string Address { get; init; }
    public required string TaxNumber { get; init; }
    public required bool IsActive { get; init; }
    public required DateTime CreatedAt { get; init; }
}
