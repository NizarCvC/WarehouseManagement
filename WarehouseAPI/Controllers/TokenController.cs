using Microsoft.AspNetCore.Mvc;
using WarehouseCore.DTOs.AuthDTOs;
using WarehouseServices.Interfaces;

namespace WarehouseAPI.Controllers;

[ApiController]
[Route("api/token")]
public class TokenController(IAuthService authService) : ControllerBase
{
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateToken(LoginDto loginDto, CancellationToken ct)
    {
        return Ok(await authService.LoginAsync(loginDto, ct));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto refreshToken, CancellationToken ct)
    {
        return Ok(await authService.RefreshTokenAsync(refreshToken, ct));
    }
}
