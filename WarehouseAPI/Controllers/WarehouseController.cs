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
[Tags("Warehouses")]
[Route("api/warehouses")]
[Produces("application/json")]
[Authorize(Roles = "System Administrator, Warehouse Manager, Purchasing Officer, Sales Representative, Accountant")]
[EnableRateLimiting(policyName: "GeneralPolicy")]
public class WarehouseController(IWarehouseService warehouseService) : ControllerBase
{
    [HttpOptions]
    [DisableRateLimiting]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [EndpointName("WarehouseOptionsV1")]
    [EndpointSummary("Get the available options in Warehouses endpoints.")]
    public IActionResult WarehouseOptions()
    {
        Response.Headers.Append("Allow", "GET, HEAD, POST, PUT, DELETE, OPTIONS");
        return NoContent();
    }

    [HttpHead("by-id/{warehouseId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("HeadWarehouseByIdV1")]
    [EndpointSummary("Check if the warehouse exists by id")]
    public async Task<IActionResult> HeadWarehouse(int warehouseId, CancellationToken ct)
    {
        return await warehouseService.IsWarehouseExistsByIdAsync(warehouseId, ct) ? Ok() : NotFound();
    }

    [HttpHead("by-name/{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("HeadWarehouseByNameV1")]
    [EndpointSummary("Check if the warehouse exists by name")]
    public async Task<IActionResult> HeadWarehouseByName(string name, CancellationToken ct)
    {
        return await warehouseService.IsWarehouseExistsByNameAsync(name, ct) ? Ok() : NotFound();
    }

    [HttpHead("by-code/{code}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("HeadWarehouseByCodeV1")]
    [EndpointSummary("Check if the warehouse exists by code")]
    public async Task<IActionResult> HeadWarehouseByCode(string code, CancellationToken ct)
    {
        return await warehouseService.IsWarehouseExistsByCodeAsync(code, ct) ? Ok() : NotFound();
    }

    [HttpGet("by-id/{warehouseId:int}", Name = "GetWarehouseById")]
    [ProducesResponseType<WarehouseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetWarehouseByIdV1")]
    [EndpointSummary("Retrieves a specific warehouse with its info")]
    [EndpointDescription("Retrieves a warehouse by ID and includes warehouse information.")]
    public async Task<ActionResult<WarehouseDto>> GetWarehouseById(int warehouseId, CancellationToken ct)
    {
        var warehouse = await warehouseService.GetWarehouseByIdAsync(warehouseId, ct);
        return Ok(warehouse);
    }

    [HttpGet("by-name/{name}", Name = "GetWarehouseByName")]
    [ProducesResponseType<WarehouseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetWarehouseByNameV1")]
    [EndpointSummary("Retrieves a specific warehouse with its info")]
    [EndpointDescription("Retrieves a warehouse by name and includes warehouse information.")]
    public async Task<ActionResult<WarehouseDto>> GetWarehouseByName(string name, CancellationToken ct)
    {
        var warehouseInfo = await warehouseService.GetWarehouseByNameAsync(name, ct);
        return Ok(warehouseInfo);
    }

    [HttpGet("by-code/{code}", Name = "GetWarehouseByCode")]
    [ProducesResponseType<WarehouseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetWarehouseByCodeV1")]
    [EndpointSummary("Retrieves a specific warehouse with its info")]
    [EndpointDescription("Retrieves a warehouse by code and includes warehouse information.")]
    public async Task<ActionResult<WarehouseDto>> GetWarehouseByCode(string code, CancellationToken ct)
    {
        var warehouseInfo = await warehouseService.GetWarehouseByCodeAsync(code, ct);
        return Ok(warehouseInfo);
    }

    [HttpGet]
    [ProducesResponseType<PagedResult<WarehouseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetPagedWarehousesV1")]
    [EndpointSummary("Retrieves paged warehouses")]
    [EndpointDescription("Retrieves warehouses using pagination by query string (page, pageSize).")]
    public async Task<IActionResult> GetPagedWarehouses(CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        int totalCount = await warehouseService.GetWarehousesCountAsync(ct);
        var warehouses = await warehouseService.GetAllWarehousesAsync(ct, page, pageSize);

        var pagedResult = PagedResult<WarehouseDto>.Create(
            warehouses,
            totalCount,
            page,
            pageSize
        );

        return Ok(pagedResult);
    }

    [HttpPost]
    [Authorize(Roles = "System Administrator, Warehouse Manager")]
    [Consumes("application/json")]
    [ProducesResponseType<CreateWarehouseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("CreateWarehouseV1")]
    [EndpointSummary("Creates a new warehouse")]
    [EndpointDescription("Registers a new warehouse into the system.")]
    public async Task<IActionResult> CreateWarehouse(CreateWarehouseDto warehouseDto, CancellationToken ct)
    {
        int newWarehouseId = await warehouseService.AddNewWarehouseAsync(warehouseDto, ct);

        return CreatedAtRoute(routeName: nameof(GetWarehouseById),
            routeValues: new { Id = newWarehouseId }, value: new { Id = newWarehouseId });
    }

    [HttpPut("{warehouseId:int}")]
    [Authorize(Roles = "System Administrator, Warehouse Manager")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("UpdateWarehouseV1")]
    [EndpointSummary("Update warehouse info")]
    [EndpointDescription("Update warehouse information.")]
    public async Task<IActionResult> UpdateWarehouse(int warehouseId, CreateWarehouseDto warehouseDto, CancellationToken ct)
    {
        await warehouseService.UpdateWarehouseAsync(warehouseId, warehouseDto, ct);
        return NoContent();
    }

    [HttpDelete("{warehouseId:int}")]
    [Authorize(Roles = "System Administrator, Warehouse Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("DeactivateWarehouseV1")]
    [EndpointSummary("Deactivate warehouse by id")]
    [EndpointDescription("Deactivate the warehouse in the system.")]
    public async Task<IActionResult> DeactivateWarehouse(int warehouseId, CancellationToken ct)
    {
        await warehouseService.DeactivateWarehouseAsync(warehouseId, ct);
        return NoContent();
    }
}
