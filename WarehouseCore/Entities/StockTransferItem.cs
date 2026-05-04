namespace WarehouseCore.Entities;

public class StockTransferItem
{
    public int StockTransferItemID { get; init; }
    public int StockTransferID { get; init; }
    public StockTransfer? StockTransfer { get; init; }
    public int ProductID { get; init; }
    public Product? Product { get; init; }
    public decimal Quantity { get; init; }
}
