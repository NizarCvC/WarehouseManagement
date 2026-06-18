using System.ComponentModel.DataAnnotations;

namespace WarehouseCore.DTOs.CreateDTOs;

public class CreateInvoiceDto
{
    [Required(ErrorMessage = "InvoiceNumber is required.")]
    [MaxLength(100, ErrorMessage = "InvoiceNumber cannot exceed 100 characters.")]
    public required string InvoiceNumber { get; set; }
    [Required(ErrorMessage = "WarehouseID is required.")]
    public required int WarehouseID { get; set; }
    [Required(ErrorMessage = "CreatedByID is required.")]
    public required int CreatedByID { get; set; }
    [Required(ErrorMessage = "Subtotal is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Subtotal must be greater than zero.")]
    public required decimal Subtotal { get; set; }
    [Required(ErrorMessage = "DiscountAmount is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "DiscountAmount must be a valid number.")]
    public required decimal DiscountAmount { get; set; }
    [Required(ErrorMessage = "TaxAmount is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "TaxAmount must be a valid number.")]
    public required decimal TaxAmount { get; set; }
    [MaxLength(500, ErrorMessage = "Note cannot exceed 500 characters.")]
    public required string? Note { get; set; }
    [Required(ErrorMessage = "At least one invoice item is required.")]
    [MinLength(1, ErrorMessage = "At least one invoice item is required.")]
    public required List<CreateInvoiceItemDto> Items { get; set; }
}
