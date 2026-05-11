using System.ComponentModel.DataAnnotations;

namespace WarehouseCore.DTOs.CreateDTOs;

public class CreateProductDto
{
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public required string Name { get; set; }
    [Required(ErrorMessage = "Sku is required.")]
    [MaxLength(100, ErrorMessage = "Sku cannot exceed 100 characters.")]
    public required string Sku { get; set; }
    [Required(ErrorMessage = "Barcode is required.")]
    [MaxLength(100, ErrorMessage = "Barcode cannot exceed 100 characters.")]
    public required string Barcode { get; set; }
    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }
    [Required(ErrorMessage = "Purchase Price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Purchase Price must be greater than 0.")]
    public required decimal PurchasePrice { get; set; }
    [Required(ErrorMessage = "Sale Price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Sale Price must be greater than 0.")]
    public required decimal SalePrice { get; set; }
    [Required(ErrorMessage = "Minimum Stock is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Minimum Stock must be a positive integer.")]
    public required int MinStock { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Category ID must be a positive integer.")]
    public int? CategoryID { get; set; }
    [Required(ErrorMessage = "Unit ID is required.")]
    [Range(1, 6, ErrorMessage = "Unit ID is not valid.")]
    public required int UnitID { get; set; }
}
