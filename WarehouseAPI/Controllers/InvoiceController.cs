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
[Route("api/invoices")]
[Authorize]
[Tags("Invoices")]
[Produces("application/json")]
[EnableRateLimiting(policyName: "GeneralPolicy")]
public class InvoiceController(IInvoiceService invoiceService) : ControllerBase
{
    [HttpOptions]
    [AllowAnonymous]
    [DisableRateLimiting]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [EndpointName("InvoiceOptionsV1")]
    [EndpointSummary("Get the available options in Invoices endpoints.")]
    public IActionResult InvoiceOptions()
    {
        Response.Headers.Append("Allow", "GET, HEAD, POST, PUT, OPTIONS");
        return NoContent();
    }

    [HttpHead("sales/{invoiceId:int}")]
    [Authorize(Roles = "System Administrator,Accountant,Sales Representative")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("HeadSalesInvoiceByIdV1")]
    [EndpointSummary("Check if the sales invoice exists by id")]
    public async Task<IActionResult> HeadSalesInvoiceById(int invoiceId, CancellationToken ct)
    {
        return await invoiceService.IsSalesInvoiceExistsById(invoiceId, ct) ? Ok() : NotFound();
    }

    [HttpHead("purchases/{invoiceId:int}")]
    [Authorize(Roles = "System Administrator,Accountant,Purchasing Officer,Warehouse Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("HeadPurchaseInvoiceByIdV1")]
    [EndpointSummary("Check if the purchase invoice exists by id")]
    public async Task<IActionResult> HeadPurchaseInvoiceById(int invoiceId, CancellationToken ct)
    {
        return await invoiceService.IsPurchaseInvoiceExistsById(invoiceId, ct) ? Ok() : NotFound();
    }

    [HttpGet("sales/{invoiceId:int}", Name = "GetSalesInvoiceById")]
    [Authorize(Roles = "System Administrator,Accountant,Sales Representative")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetSalesInvoiceByIdV1")]
    [EndpointSummary("Retrieves a specific sales invoice with its info")]
    [EndpointDescription("Retrieves a sales invoice by ID and includes invoice information.")]
    public async Task<ActionResult<SalesInvoiceDetailsDto>> GetSalesInvoiceById(int invoiceId, CancellationToken ct)
    {
        var salesInvoiceInfo = await invoiceService.GetSalesInvoiceByIdAsync(invoiceId, ct);
        return Ok(salesInvoiceInfo);
    }

    [HttpGet("purchases/{invoiceId:int}", Name = "GetPurchaseInvoiceById")]
    [Authorize(Roles = "System Administrator,Accountant,Purchasing Officer,Warehouse Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetPurchaseInvoiceByIdV1")]
    [EndpointSummary("Retrieves a specific purchase invoice with its info")]
    [EndpointDescription("Retrieves a purchase invoice by ID and includes invoice information.")]
    public async Task<ActionResult<PurchaseInvoiceDetailsDto>> GetPurchaseInvoiceById(int invoiceId, CancellationToken ct)
    {
        var purchaseInvoiceInfo = await invoiceService.GetPurchaseInvoiceByIdAsync(invoiceId, ct);
        return Ok(purchaseInvoiceInfo);
    }

    [HttpPost("sales")]
    [Authorize(Roles = "System Administrator,Sales Representative")] 
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("CreateSalesInvoiceV1")]
    [EndpointSummary("Creates a new sales invoice")]
    [EndpointDescription("Creates a new sales invoice with the provided details.")]
    public async Task<IActionResult> CreateSalesInvoice(CreateSalesInvoiceDto salesInvoiceDto, CancellationToken ct)
    {
        int newInvoiceId = await invoiceService.CreateSalesInvoiceAsync(salesInvoiceDto, ct);

        
        return CreatedAtRoute(routeName: nameof(GetSalesInvoiceById),
            routeValues: new { invoiceId = newInvoiceId }, value: new { Id = newInvoiceId });
    }

    [HttpPost("purchases")]
    [Authorize(Roles = "System Administrator,Purchasing Officer")] 
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("CreatePurchaseInvoiceV1")]
    [EndpointSummary("Creates a new purchase invoice")]
    [EndpointDescription("Creates a new purchase invoice with the provided details.")]
    public async Task<IActionResult> CreatePurchaseInvoice(CreatePurchaseInvoiceDto purchaseInvoiceDto, CancellationToken ct)
    {
        int newInvoiceId = await invoiceService.CreatePurchaseInvoiceAsync(purchaseInvoiceDto, ct);

        
        return CreatedAtRoute(routeName: nameof(GetPurchaseInvoiceById),
            routeValues: new { invoiceId = newInvoiceId }, value: new { Id = newInvoiceId });
    }

    [HttpPut("status")]
    [Authorize(Roles = "System Administrator,Accountant")] 
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("UpdateInvoiceStatusV1")]
    [EndpointSummary("Updates the status of a specific invoice")]
    [EndpointDescription("Updates the status of an existing invoice.")]
    public async Task<IActionResult> UpdateInvoiceStatus([FromQuery] int invoiceId, [FromQuery] int newStatusId, CancellationToken ct)
    {
        await invoiceService.UpdateInvoiceStatusAsync(invoiceId, newStatusId, ct);
        return NoContent();
    }

    [HttpGet("sales/customer/{customerId:int}")]
    [Authorize(Roles = "System Administrator,Accountant,Sales Representative")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetCustomerInvoicesV1")]
    [EndpointSummary("Retrieves invoices for a specific customer")]
    [EndpointDescription("Retrieves a list of invoices for the specified customer.")]
    public async Task<IActionResult> GetCustomerInvoices(int customerId, CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        int totalCount = await invoiceService.GetCustomerInvoicesCountAsync(customerId, ct);
        var invoices = await invoiceService.GetCustomerInvoicesAsync(customerId, ct, page, pageSize);

        var pagedResult = PagedResult<InvoiceSummaryDto>.Create(
            invoices,
            totalCount,
            page,
            pageSize
        );

        return Ok(pagedResult);
    }

    [HttpGet("purchases/supplier/{supplierId:int}")]
    [Authorize(Roles = "System Administrator,Accountant,Purchasing Officer,Warehouse Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetSupplierInvoicesV1")]
    [EndpointSummary("Retrieves invoices for a specific supplier")]
    [EndpointDescription("Retrieves a list of invoices for the specified supplier.")]
    public async Task<IActionResult> GetSupplierInvoices(int supplierId, CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        int totalCount = await invoiceService.GetSupplierInvoicesCountAsync(supplierId, ct);
        
        
        var invoices = await invoiceService.GetSupplierInvoicesAsync(supplierId, ct, page, pageSize);

        var pagedResult = PagedResult<InvoiceSummaryDto>.Create(
            invoices,
            totalCount,
            page,
            pageSize
        );

        return Ok(pagedResult);
    }

    [HttpGet]
    [Authorize(Roles = "System Administrator,Accountant")] 
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetInvoicesSummaryV1")]
    [EndpointSummary("Retrieves a summary of all invoices")]
    [EndpointDescription("Retrieves a summary of all invoices in the system.")]
    public async Task<IActionResult> GetInvoicesSummary(CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        int totalCount = await invoiceService.GetInvoicesSummaryCountAsync(ct);
        var invoices = await invoiceService.GetInvoicesSummaryAsync(ct, page, pageSize);

        var pagedResult = PagedResult<InvoiceSummaryDto>.Create(
            invoices,
            totalCount,
            page,
            pageSize
        );

        return Ok(pagedResult);
    }
}