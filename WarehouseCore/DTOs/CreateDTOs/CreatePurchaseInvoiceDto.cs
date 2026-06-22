using System.ComponentModel.DataAnnotations;

namespace WarehouseCore.DTOs.CreateDTOs;

public class CreatePurchaseInvoiceDto : CreateInvoiceDto
{
    [Required(ErrorMessage = "SupplierID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "SupplierID must be a positive integer.")]
    public required int SupplierID { get; set; }
}
