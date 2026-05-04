namespace WarehouseCore.Entities;

public class StockTransfer
{
    public int StockTransferID { get; init; }
    public required string TransferNumber { get; init; }
    public DateTime TransferDate { get; init; }
    public byte StatusID { get; init; }
    public TransferStatus? TransferStatus { get; init; }
    public string? Note { get; init; }
    public int FromWarehouseID { get; init; }
    public int ToWarehouseID { get; init; }
    public int CreatedByID { get; init; }
    public List<StockTransferItem>? StockTransferItems;
}
