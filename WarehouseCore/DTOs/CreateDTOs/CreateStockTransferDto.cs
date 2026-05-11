namespace WarehouseCore.DTOs.CreateDTOs;

public class CreateStockTransferDto
{
    public required string TransferNumber { get; set; }
    public required int SourceWarehouseID { get; set; }
    public required int DestinationWarehouseID { get; set; }
    public required int CreatedByID { get; set; }
    public required string? Note { get; set; }

    public required List<CreateTransferItemDto> Items { get; set; }
}