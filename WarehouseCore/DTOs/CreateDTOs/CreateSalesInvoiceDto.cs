using System.ComponentModel.DataAnnotations;

namespace WarehouseCore.DTOs.CreateDTOs;

public class CreateSalesInvoiceDto : CreateInvoiceDto
{
    [Required(ErrorMessage = "CustomerID is required.")]
    public required int CustomerID { get; set; }
}
