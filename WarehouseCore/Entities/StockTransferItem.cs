namespace WarehouseCore.Entities;

public class StockTransferItem
{
    public required int StockTransferItemID { get; init; }
    public required int StockTransferID { get; init; }
    public StockTransfer StockTransfer { get; init; } = null!;
    public required int ProductID { get; init; }
    public Product Product { get; init; } = null!;
    public required decimal Quantity { get; init; }
}
