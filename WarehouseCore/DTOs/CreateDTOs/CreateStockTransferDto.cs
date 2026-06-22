using System.ComponentModel.DataAnnotations;

namespace WarehouseCore.DTOs.CreateDTOs;

public class CreateStockTransferDto
{
    [Required(ErrorMessage = "TransferNumber is required.")]
    [MaxLength(100, ErrorMessage = "TransferNumber cannot exceed 100 characters.")]
    public required string TransferNumber { get; set; }
    [Required(ErrorMessage = "SourceWarehouseID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "SourceWarehouseID must be a positive integer.")]
    public required int SourceWarehouseID { get; set; }
    [Required(ErrorMessage = "DestinationWarehouseID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "DestinationWarehouseID must be a positive integer.")]
    public required int DestinationWarehouseID { get; set; }
    [Required(ErrorMessage = "CreatedByID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "CreatedByID must be a positive integer.")]
    public required int CreatedByID { get; set; }
    [MaxLength(500, ErrorMessage = "Note cannot exceed 500 characters.")]
    public required string? Note { get; set; }
    [Required(ErrorMessage = "At least one stock item is required.")]
    [MinLength(1, ErrorMessage = "At least one stock item is required.")]
    public required List<CreateTransferItemDto> Items { get; set; }
}