namespace WarehouseCore.Entities;

public class InventoryTransaction
{
    public int InventoryTransactionID { get; init; }
    public bool IsInward { get; init; }
    public decimal Quantity { get; init; }
    public decimal UnitCost { get; init; }
    public int ReferenceID { get; init; }
    public byte ReferenceTypeID { get; init; }
    public required ReferenceType ReferenceType { get; init; }
    public string? Note { get; init; }
    public int ProductID { get; init; }
    public required Product Product { get; init; }
    public int WarehouseID { get; init; }
    public required Warehouse Warehouse { get; init; }
    public int CreatedByID { get; init; }
    public required User CreatedBy { get; init; }
    public DateTime CreatedAt { get; init; }
}