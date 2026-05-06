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
    [HttpOptions]
    public IActionResult UserOptions()
    {
        Response.Headers.Append("Allow", "GET, HEAD, POST, PUT, DELETE, OPTIONS");
        return NoContent();
    }

    [HttpHead("by-id/{userId:int}")]
    public async Task<IActionResult> HeadUser(int userId, CancellationToken ct)
    {
        return await userService.IsUserIdExists(userId, ct) ? Ok() : NotFound();
    }

    [HttpHead("by-username/{username}")]
    public async Task<IActionResult> HeadUser(string username, CancellationToken ct)
    {
        return await userService.IsUsernameExists(username, ct) ? Ok() : NotFound();
    }

    [HttpGet("by-id/{userId:int}", Name = "GetUserById")]
    public async Task<ActionResult<UserDto>> GetUserById(int userId, CancellationToken ct)
    {
        var user = await userService.GetUserByIdAsync(userId, ct);
        return Ok(user);
    }

    [HttpGet("by-username/{username}", Name = "GetByUsername")]
    public async Task<ActionResult<UserDto>> GetUserByUsername(string username, CancellationToken ct)
    {
        var userInfo = await userService.GetUserByUsernameAsync(username, ct);
        return Ok(userInfo);
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
        int newUserId = await userService.AddNewUserAsync(userDto, ct);

        return CreatedAtRoute(routeName: nameof(GetUserById),
            routeValues: new { UserId = newUserId }, value: userDto);
    }

    [HttpPut("{userId:int}")]
    public async Task<IActionResult> UpdateUser(int userId, CreateUserDto userDto, CancellationToken ct)
    {
        await userService.UpdateUserAsync(userId, userDto, ct);
        return NoContent();
    }

    [HttpDelete("{userId:int}")]
    public async Task<IActionResult> DeleteUser(int userId, CancellationToken ct)
    {
        await userService.DeleteUserAsync(userId, ct);
        return NoContent();
    }

}
