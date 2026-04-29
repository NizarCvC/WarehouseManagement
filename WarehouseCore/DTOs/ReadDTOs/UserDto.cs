namespace WarehouseCore.DTOs.ReadDTOs;

public class UserDto
{
    public int UserID { get; set; }
    public required string Name { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public bool IsActive { get; set; }
    public required string RoleName { get; set; } 
    public DateTime CreatedAt { get; set; }
}