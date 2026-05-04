using M02.BuildingRESTFulAPI.Responses;
using Microsoft.AspNetCore.Mvc;
using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseServices.Interfaces;

namespace WarehouseAPI.Controllers;

[ApiController]
[Route("api/Users")]
public class UserController(IUserService userService) : ControllerBase
{
    public IActionResult UserOptions()
    {
        Response.Headers.Append("Allow", "GET, HEAD, POST, PUT, DELETE, OPTIONS");
        return NoContent();
    }

    [HttpHead("{userId:int}")]
    public async Task<IActionResult> HeadUser(int userId, CancellationToken ct)
    {
        return await userService.IsUserIdExists(userId, ct) ? Ok() : NotFound();
    }

    [HttpHead("{username:alpha}")]
    public async Task<IActionResult> HeadUser(string username, CancellationToken ct)
    {
        return await userService.IsUsernameExists(username, ct) ? Ok() : NotFound();
    }
    
    [HttpGet("{userId:int}", Name = "GetUserById")]
    public async Task<ActionResult<UserDto>> GetUserById(int userId, CancellationToken ct)
    {
        var user = await userService.GetUserByIdAsync(userId, ct);

        if (user is null)
            return NotFound($"The user with ID: {userId} not exists.");

        return Ok(user);
    }

    [HttpGet]
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
    public async Task<IActionResult> CreateUser(CreateUserDto userDto, CancellationToken ct)
    {
        if (await userService.IsUsernameExists(userDto.Username, ct))
            return Conflict($"The user with username {userDto.Username} is already used.");

        int newUserId = await userService.AddNewUserAsync(userDto, ct);

        return CreatedAtRoute(routeName: nameof(GetUserById),
            routeValues: new { UserId = newUserId }, value: userDto);
    }

    [HttpPut("{userId:int}")]
    public async Task<IActionResult> UpdateUser(int userId, CreateUserDto userDto, CancellationToken ct)
    {
        var user = await userService.GetUserByIdAsync(userId, ct);

        if (user is null)
            return NotFound($"The user with ID: {userId} not exists.");

        bool isUsernameUsed = await userService.IsUsernameExists(userDto.Username, ct);
        
        if (user.Username != userDto.Username && isUsernameUsed)
            return Conflict($"The username: {userDto.Username} is already used.");

        bool isSeccessed = await userService.UpdateUserAsync(userId, userDto, ct);

        if (!isSeccessed)
            return StatusCode(500, "Failed to update user");
        
        return NoContent();
    }

    [HttpDelete("{userId:int}")]
    public async Task<IActionResult> DeleteUser(int userId, CancellationToken ct)
    {
        if (!await userService.IsUserIdExists(userId, ct))
            return NotFound($"The user with ID: {userId} not exists.");

        bool isSeccessed = await userService.DeleteUserAsync(userId, ct);

        if (!isSeccessed)
            return StatusCode(500, "Failed to delete user");
        
        return NoContent();
    }

}
