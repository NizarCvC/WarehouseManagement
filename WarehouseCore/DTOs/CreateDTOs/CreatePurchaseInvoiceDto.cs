using System.ComponentModel.DataAnnotations;

namespace WarehouseCore.DTOs.CreateDTOs;

public class CreatePurchaseInvoiceDto : CreateInvoiceDto
{
    [Required(ErrorMessage = "SupplierID is required.")]
    public required int SupplierID { get; set; }
}
