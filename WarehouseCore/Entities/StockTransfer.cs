namespace WarehouseCore.Entities;

public class StockTransfer
{
    public int StockTransferID { get; init; }
    public required string TransferNumber { get; init; }
    public DateTime TransferDate { get; init; }
    public byte StatusID { get; init; }
    public TransferStatus TransferStatus { get; init; } = null!;
    public string? Note { get; init; }
    public int FromWarehouseID { get; init; }
    public Warehouse FromWarehouse { get; init; } = null!;
    public int ToWarehouseID { get; init; }
    public Warehouse ToWarehouse { get; init; } = null!;
    public int CreatedByID { get; init; }
    public User CreatedBy { get; init; } = null!;
    public ICollection<StockTransferItem> StockTransferItems { get; init; } = [];
}
