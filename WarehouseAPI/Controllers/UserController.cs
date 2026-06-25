using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WarehouseAPI.Helpers;
using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseServices.Interfaces;

namespace WarehouseAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/Users")]
[Authorize(Policy = "System Administrator")]
[Tags("Users")]
[Produces("application/json")]
[EnableRateLimiting(policyName: "GeneralPolicy")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpOptions]
    [AllowAnonymous]
    [DisableRateLimiting]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [EndpointName("UserOptionsV1")]
    [EndpointSummary("Get the available options in Users endpoints.")]
    public IActionResult UserOptions()
    {
        Response.Headers.Append("Allow", "GET, HEAD, POST, PUT, DELETE, OPTIONS");
        return NoContent();
    }

    [HttpHead("by-id/{userId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("HeadUserByIdV1")]
    [EndpointSummary("Check if the user exists by id")]
    public async Task<IActionResult> HeadUser(int userId, CancellationToken ct)
    {
        return await userService.IsUserIdExistsAsync(userId, ct) ? Ok() : NotFound();
    }

    [HttpHead("by-username/{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("HeadUserByUsernameV1")]
    [EndpointSummary("Check if the user exists by username")]
    public async Task<IActionResult> HeadUser(string username, CancellationToken ct)
    {
        return await userService.IsUsernameExistsAsync(username, ct) ? Ok() : NotFound();
    }

    [HttpGet("by-id/{userId:int}", Name = "GetUserById")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetUserByIdV1")]
    [EndpointSummary("Retrieves a specific user with his info")]
    [EndpointDescription("Retrieves a user by ID and includes user information.")]
    public async Task<ActionResult<UserDto>> GetUserById(int userId, CancellationToken ct)
    {
        var user = await userService.GetUserByIdAsync(userId, ct);
        return Ok(user);
    }

    [HttpGet("by-username/{username}", Name = "GetByUsername")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetByUsernameV1")]
    [EndpointSummary("Retrieves a specific user with his info")]
    [EndpointDescription("Retrieves a user by username and includes user information.")]
    public async Task<ActionResult<UserDto>> GetUserByUsername(string username, CancellationToken ct)
    {
        var userInfo = await userService.GetUserByUsernameAsync(username, ct);
        return Ok(userInfo);
    }

    [HttpGet]
    [ProducesResponseType<PagedResult<UserDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetPagedUsersV1")]
    [EndpointSummary("Retrieves paged users")]
    [EndpointDescription("Retrieves users using pagination by query string (page, pageSize).")]
    public async Task<IActionResult> GetPagedUsers(CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        int totalCount = await userService.GetUsersCountAsync(ct);
        var users = await userService.GetAllUsersAsync(ct, page, pageSize);

        var pagedResult = PagedResult<UserDto>.Create(
            users,
            totalCount,
            page,
            pageSize
        );

        return Ok(pagedResult);
    }

    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType<CreateUserDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("CreateUserV1")]
    [EndpointSummary("Creates a new user")]
    [EndpointDescription("Registers a new user into the system.")]
    public async Task<IActionResult> CreateUser(CreateUserDto userDto, CancellationToken ct)
    {
        int newUserId = await userService.AddNewUserAsync(userDto, ct);

        return CreatedAtRoute(routeName: nameof(GetUserById),
            routeValues: new { UserId = newUserId }, value: new { UserId = newUserId });
    }

    [HttpPut("{userId:int}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("UpdateUserV1")]
    [EndpointSummary("Update user info")]
    [EndpointDescription("Update user information.")]
    public async Task<IActionResult> UpdateUser(int userId, CreateUserDto userDto, CancellationToken ct)
    {
        await userService.UpdateUserAsync(userId, userDto, ct);
        return NoContent();
    }

    [HttpDelete("{userId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("DeactivateUserV1")]
    [EndpointSummary("Deactivate user by id")]
    [EndpointDescription("Deactivate the user in the system.")]
    public async Task<IActionResult> DeactivateUser(int userId, CancellationToken ct)
    {
        await userService.DeactivateUserAsync(userId, ct);
        return NoContent();
    }

}