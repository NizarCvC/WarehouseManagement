namespace WarehouseCore.Entities;

public class StockTransfer
{
    public int StockTransferID { get; set; }
    public required string TransferNumber { get; set; }
    public DateTime TransferDate { get; set; }
    public byte StatusID { get; set; }
    public string? Note { get; set; }
    public int FromWarehouseID { get; set; }
    public int ToWarehouseID { get; set; }
    public int CreatedByID { get; set; }
}
