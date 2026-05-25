namespace WarehouseCore.Entities;

public class StockBalance
{
    public required int StockBalanceID { get; init; }
    public required decimal Quantity { get; init; }
    public required int ProductID { get; init; }
    public Product Product { get; init; } = null!;
    public required int WarehouseID { get; init; }
    public Warehouse Warehouse { get; init; } = null!;
}
