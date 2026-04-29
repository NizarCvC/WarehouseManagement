namespace WarehouseCore.DTOs.CreateDTOs;

public class CreateStockTransferDto
{
    public required string TransferNumber { get; set; }
    public int SourceWarehouseID { get; set; }
    public int DestinationWarehouseID { get; set; }
    public int CreatedByID { get; set; }
    public string? Note { get; set; }

    public required List<CreateTransferItemDto> Items { get; set; }
}