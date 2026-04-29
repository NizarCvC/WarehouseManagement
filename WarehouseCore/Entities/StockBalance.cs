namespace WarehouseCore.Entities;

public class StockBalance
{
    public int StockBalanceID { get; set; }
    public decimal Quantity { get; set; }
    public int ProductID { get; set; }
    public int WarehouseID { get; set; }
}
