namespace WarehouseCore.DTOs.AuthDTOs;

public class AuthResponseDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public DateTime Expires { get; set; }
}
