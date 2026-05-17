namespace WarehouseCore.Entities;

public class StockTransferItem
{
    public int StockTransferItemID { get; init; }
    public int StockTransferID { get; init; }
    public StockTransfer StockTransfer { get; init; } = null!;
    public int ProductID { get; init; }
    public Product Product { get; init; } = null!;
    public decimal Quantity { get; init; }
}
