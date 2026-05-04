namespace WarehouseCore.Entities;

public class StockBalance
{
    public int StockBalanceID { get; init; }
    public decimal Quantity { get; init; }
    public int ProductID { get; init; }
    public required Product Product { get; init; }
    public int WarehouseID { get; init; }
    public required Warehouse Warehouse { get; init; }
}
