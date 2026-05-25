using WarehouseCore.enums;

namespace WarehouseCore.Entities;

public class StockTransfer
{
    public required int StockTransferID { get; init; }
    public required string TransferNumber { get; init; }
    public required DateTime TransferDate { get; init; }
    public required byte StatusID { get; init; }
    public enTransferStatus TransferStatus { get => (enTransferStatus)StatusID; }
    public string? Note { get; init; }
    public required int FromWarehouseID { get; init; }
    public Warehouse FromWarehouse { get; init; } = null!;
    public required int ToWarehouseID { get; init; }
    public Warehouse ToWarehouse { get; init; } = null!;
    public required int CreatedByID { get; init; }
    public User CreatedBy { get; init; } = null!;
    public ICollection<StockTransferItem> StockTransferItems { get; init; } = [];
}
