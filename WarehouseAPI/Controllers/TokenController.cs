using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using WarehouseCore.DTOs.AuthDTOs;
using WarehouseServices.Interfaces;

namespace WarehouseAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/token")] 
[Tags("Authentication")] 
[Produces("application/json")]
public class TokenController(IAuthService authService) : ControllerBase
{
    [HttpPost("generate")]
    [Consumes("application/json")]
    [ProducesResponseType<AuthResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)] 
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GenerateTokenV1")]
    [EndpointSummary("Generate access and refresh tokens")]
    [EndpointDescription("Authenticates a user using their credentials and returns a pair of JWT access and refresh tokens.")]
    public async Task<IActionResult> GenerateToken([FromBody] LoginDto loginDto, CancellationToken ct)
    {
        var result = await authService.LoginAsync(loginDto, ct);
        return Ok(result);
    }

    [HttpPost("refresh")]
    [Consumes("application/json")]
    [ProducesResponseType<AuthResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)] 
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)] 
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("RefreshTokenV1")]
    [EndpointSummary("Refresh an expired access token")]
    [EndpointDescription("Takes an expired access token and a valid refresh token to issue a new pair of access and refresh tokens.")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto refreshToken, CancellationToken ct)
    {
        var result = await authService.RefreshTokenAsync(refreshToken, ct);
        return Ok(result);
    }
}