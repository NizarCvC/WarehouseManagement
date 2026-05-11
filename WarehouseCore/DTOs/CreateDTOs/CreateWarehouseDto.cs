using System.ComponentModel.DataAnnotations;

namespace WarehouseCore.DTOs.CreateDTOs;

public class CreateWarehouseDto
{
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public required string Name { get; set; }
    [Required(ErrorMessage = "Code is required.")]
    [MaxLength(100, ErrorMessage = "Code cannot exceed 100 characters.")]
    public required string Code { get; set; }
    [Required(ErrorMessage = "Location is required.")]
    [MaxLength(500, ErrorMessage = "Location cannot exceed 500 characters.")]
    public required string Location { get; set; }
}
