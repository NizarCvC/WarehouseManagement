using System.ComponentModel.DataAnnotations;

namespace WarehouseCore.DTOs.CreateDTOs;

public class CreateTransferItemDto
{
    [Required(ErrorMessage = "ProductID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "ProductID must be a positive integer.")]
    public required int ProductID { get; set; }
    [Required(ErrorMessage = "Quantity is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
    public required decimal Quantity { get; set; }
} 
