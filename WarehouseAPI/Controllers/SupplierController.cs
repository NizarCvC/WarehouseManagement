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
[Route("api/suppliers")]
[Authorize(Roles = "System Administrator, Purchasing Officer, Warehouse Manager, Accountant")]
[Tags("Suppliers")]
[Produces("application/json")]
[EnableRateLimiting(policyName: "GeneralPolicy")]
public class SupplierController(ISupplierService supplierService) : ControllerBase
{
    [HttpOptions]
    [AllowAnonymous]
    [DisableRateLimiting]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [EndpointName("SupplierOptionsV1")]
    [EndpointSummary("Get the available options in Suppliers endpoints.")]
    public IActionResult SupplierOptions()
    {
        Response.Headers.Append("Allow", "GET, HEAD, POST, PUT, DELETE, OPTIONS");
        return NoContent();
    }

    [HttpHead("by-id/{supplierId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("HeadSupplierByIdV1")]
    [EndpointSummary("Check if the supplier exists by id")]
    public async Task<IActionResult> HeadSupplier(int supplierId, CancellationToken ct)
    {
        return await supplierService.IsSupplierExistsByIdAsync(supplierId, ct) ? Ok() : NotFound();
    }

    [HttpHead("by-email/{email}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("HeadSupplierByEmailV1")]
    [EndpointSummary("Check if the supplier exists by email")]
    public async Task<IActionResult> HeadSupplierByEmail(string email, CancellationToken ct)
    {
        return await supplierService.IsSupplierExistsByEmailAsync(email, ct) ? Ok() : NotFound();
    }

    [HttpHead("by-phone/{phone}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("HeadSupplierByPhoneV1")]
    [EndpointSummary("Check if the supplier exists by phone")]
    public async Task<IActionResult> HeadSupplierByPhone(string phone, CancellationToken ct)
    {
        return await supplierService.IsSupplierExistsByPhoneAsync(phone, ct) ? Ok() : NotFound();
    }

    [HttpHead("by-tax-number/{taxNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("HeadSupplierByTaxNumberV1")]
    [EndpointSummary("Check if the supplier exists by tax number")]
    public async Task<IActionResult> HeadSupplierByTaxNumber(string taxNumber, CancellationToken ct)
    {
        return await supplierService.IsSupplierExistsByTaxNumberAsync(taxNumber, ct) ? Ok() : NotFound();
    }

    [HttpGet("by-id/{supplierId:int}", Name = "GetSupplierById")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetSupplierByIdV1")]
    [EndpointSummary("Retrieves a specific supplier with its info")]
    [EndpointDescription("Retrieves a supplier by ID and includes supplier information.")]
    public async Task<ActionResult<SupplierDto>> GetSupplierById(int supplierId, CancellationToken ct)
    {
        var supplier = await supplierService.GetSupplierByIdAsync(supplierId, ct);
        return Ok(supplier);
    }

    [HttpGet("by-email/{email}", Name = "GetSupplierByEmail")]
    [ProducesResponseType<SupplierDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetSupplierByEmailV1")]
    [EndpointSummary("Retrieves a specific supplier with its info")]
    [EndpointDescription("Retrieves a supplier by email and includes supplier information.")]
    public async Task<ActionResult<SupplierDto>> GetSupplierByEmail(string email, CancellationToken ct)
    {
        var supplierInfo = await supplierService.GetSupplierByEmailAsync(email, ct);
        return Ok(supplierInfo);
    }

    [HttpGet("by-phone/{phone}", Name = "GetSupplierByPhone")]
    [ProducesResponseType<SupplierDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetSupplierByPhoneV1")]
    [EndpointSummary("Retrieves a specific supplier with its info")]
    [EndpointDescription("Retrieves a supplier by phone and includes supplier information.")]
    public async Task<ActionResult<SupplierDto>> GetSupplierByPhone(string phone, CancellationToken ct)
    {
        var supplierInfo = await supplierService.GetSupplierByPhoneAsync(phone, ct);
        return Ok(supplierInfo);
    }

    [HttpGet("by-tax-number/{taxNumber}", Name = "GetSupplierByTaxNumber")]
    [ProducesResponseType<SupplierDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetSupplierByTaxNumberV1")]
    [EndpointSummary("Retrieves a specific supplier with its info")]
    [EndpointDescription("Retrieves a supplier by tax number and includes supplier information.")]
    public async Task<ActionResult<SupplierDto>> GetSupplierByTaxNumber(string taxNumber, CancellationToken ct)
    {
        var supplierInfo = await supplierService.GetSupplierByTaxNumberAsync(taxNumber, ct);
        return Ok(supplierInfo);
    }

    [HttpGet]
    [ProducesResponseType<PagedResult<SupplierDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetPagedSuppliersV1")]
    [EndpointSummary("Retrieves paged suppliers")]
    [EndpointDescription("Retrieves suppliers using pagination by query string (page, pageSize).")]
    public async Task<IActionResult> GetPagedSuppliers(CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        int totalCount = await supplierService.GetSuppliersCountAsync(ct);
        var suppliers = await supplierService.GetAllSuppliersAsync(ct, page, pageSize);

        var pagedResult = PagedResult<SupplierDto>.Create(
            suppliers,
            totalCount,
            page,
            pageSize
        );

        return Ok(pagedResult);
    }

    [HttpPost]
    [Authorize(Roles = "System Administrator, Purchasing Officer")]
    [Consumes("application/json")]
    [ProducesResponseType<CreateSupplierDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("CreateSupplierV1")]
    [EndpointSummary("Creates a new supplier")]
    [EndpointDescription("Registers a new supplier into the system.")]
    public async Task<IActionResult> CreateSupplier(CreateSupplierDto supplierDto, CancellationToken ct)
    {
        int newSupplierId = await supplierService.AddNewSupplierAsync(supplierDto, ct);

        return CreatedAtRoute(routeName: nameof(GetSupplierById),
            routeValues: new { Id = newSupplierId }, value: new { Id = newSupplierId });
    }

    [HttpPut("{supplierId:int}")]
    [Authorize(Roles = "System Administrator, Purchasing Officer")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("UpdateSupplierV1")]
    [EndpointSummary("Update supplier info")]
    [EndpointDescription("Update supplier information.")]
    public async Task<IActionResult> UpdateSupplier(int supplierId, CreateSupplierDto supplierDto, CancellationToken ct)
    {
        await supplierService.UpdateSupplierAsync(supplierId, supplierDto, ct);
        return NoContent();
    }

    [HttpDelete("{supplierId:int}")]
    [Authorize(Roles = "System Administrator, Purchasing Officer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("DeactivateSupplierV1")]
    [EndpointSummary("Deactivate supplier by id")]
    [EndpointDescription("Deactivate the supplier in the system.")]
    public async Task<IActionResult> DeactivateSupplier(int supplierId, CancellationToken ct)
    {
        await supplierService.DeactivateSupplierAsync(supplierId, ct);
        return NoContent();
    }
}
