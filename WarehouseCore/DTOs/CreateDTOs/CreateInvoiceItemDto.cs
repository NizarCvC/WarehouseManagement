using System.ComponentModel.DataAnnotations;

namespace WarehouseCore.DTOs.CreateDTOs;

public class CreateInvoiceItemDto
{
    [Required(ErrorMessage = "ProductID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "ProductID must be a positive integer.")]
    public required int ProductID { get; set; }
    [Required(ErrorMessage = "Quantity is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
    public required decimal Quantity { get; set; }
    [Required(ErrorMessage = "UnitPrice is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "UnitPrice must be greater than zero.")]
    public required decimal UnitPrice { get; set; }
    [Required(ErrorMessage = "DiscountAmount is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "DiscountAmount must be a positive number.")]
    public required decimal DiscountAmount { get; set; }
    [Required(ErrorMessage = "TaxAmount is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "TaxAmount must be a positive number.")]
    public required decimal TaxAmount { get; set; }
}
