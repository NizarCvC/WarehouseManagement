using WarehouseCore.enums;

namespace WarehouseCore.Entities;

public class InventoryTransaction
{
    public required int InventoryTransactionID { get; init; }
    public required bool IsInward { get; init; }
    public required decimal Quantity { get; init; }
    public required decimal UnitCost { get; init; }
    public required int ReferenceID { get; init; }
    public required byte ReferenceTypeID { get; init; }
    public enReferenceType ReferenceType { get => (enReferenceType)ReferenceTypeID; }
    public string? Note { get; init; }
    public required int ProductID { get; init; }
    public Product Product { get; init; } = null!;
    public required int WarehouseID { get; init; }
    public Warehouse Warehouse { get; init; } = null!;
    public required int CreatedByID { get; init; }
    public User CreatedBy { get; init; } = null!;
    public required DateTime CreatedAt { get; init; }
}