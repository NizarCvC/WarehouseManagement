namespace WarehouseCore.Entities;

public class StockTransferItem
{
    public int StockTransferItemID { get; set; }
    public int StockTransferID { get; set; }
    public int ProductID { get; set; }
    public decimal Quantity { get; set; }
}
