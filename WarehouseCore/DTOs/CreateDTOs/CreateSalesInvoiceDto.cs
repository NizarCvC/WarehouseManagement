using System.ComponentModel.DataAnnotations;

namespace WarehouseCore.DTOs.CreateDTOs;

public class CreateSalesInvoiceDto : CreateInvoiceDto
{
    [Required(ErrorMessage = "CustomerID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "CustomerID must be a positive integer.")]
    public required int CustomerID { get; set; }
}
