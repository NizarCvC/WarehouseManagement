namespace WarehouseCore.Entities;

public class InventoryTransaction
{
    public int InventoryTransactionID { get; init; }
    public bool IsInward { get; init; }
    public decimal Quantity { get; init; }
    public decimal UnitCost { get; init; }
    public int ReferenceID { get; init; }
    public byte ReferenceTypeID { get; init; }
    public ReferenceType ReferenceType { get; init; } = null!;
    public string? Note { get; init; }
    public int ProductID { get; init; }
    public Product Product { get; init; } = null!;
    public int WarehouseID { get; init; }
    public Warehouse Warehouse { get; init; } = null!;
    public int CreatedByID { get; init; }
    public User CreatedBy { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
}