namespace WarehouseCore.Entities;

public class StockBalance
{
    public int StockBalanceID { get; init; }
    public decimal Quantity { get; init; }
    public int ProductID { get; init; }
    public Product Product { get; init; } = null!;
    public int WarehouseID { get; init; }
    public Warehouse Warehouse { get; init; } = null!;
}
