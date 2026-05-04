namespace WarehouseCore.Entities;

public class StockTransferItem
{
    public int StockTransferItemID { get; init; }
    public int StockTransferID { get; init; }
    public required StockTransfer StockTransfer { get; init; }
    public int ProductID { get; init; }
    public required Product Product { get; init; }
    public decimal Quantity { get; init; }
}
