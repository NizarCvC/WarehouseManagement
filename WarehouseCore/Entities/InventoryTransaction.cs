namespace WarehouseCore.Entities;

public class InventoryTransaction
{
    public int InventoryTransactionID { get; set; }
    public bool IsInward { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public int ReferenceID { get; set; }
    public byte ReferenceTypeID { get; set; }
    public string? Note { get; set; }
    public int ProductID { get; set; }
    public int WarehouseID { get; set; }
    public int CreatedByID { get; set; }
    public DateTime CreatedAt { get; set; }
}