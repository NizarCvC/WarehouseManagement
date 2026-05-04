namespace WarehouseCore.Entities;

public class StockTransfer
{
    public int StockTransferID { get; init; }
    public required string TransferNumber { get; init; }
    public DateTime TransferDate { get; init; }
    public byte StatusID { get; init; }
    public required TransferStatus TransferStatus { get; init; }
    public string? Note { get; init; }
    public int FromWarehouseID { get; init; }
    public required Warehouse FromWarehouse { get; init; }
    public int ToWarehouseID { get; init; }
    public required Warehouse ToWarehouse { get; init; }
    public int CreatedByID { get; init; }
    public required User CreatedBy { get; init; }
    public List<StockTransferItem>? StockTransferItems = default;
}
