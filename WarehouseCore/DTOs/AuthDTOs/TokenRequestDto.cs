namespace WarehouseCore.DTOs.AuthDTOs;

public class TokenRequestDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public DateTime Expires { get; set; }
}