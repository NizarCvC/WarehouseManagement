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
[Route("api/customers")]
[Authorize(Roles = "System Administrator, Sales Representative, Warehouse Manager, Accountant")]
[Tags("Customers")]
[Produces("application/json")]
[EnableRateLimiting(policyName: "GeneralPolicy")]
public class CustomerController(ICustomerService customerService) : ControllerBase
{
    [HttpOptions]
    [DisableRateLimiting]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [EndpointName("CustomerOptionsV1")]
    [EndpointSummary("Get the available options in Customers endpoints.")]
    public IActionResult CustomerOptions()
    {
        Response.Headers.Append("Allow", "GET, HEAD, POST, PUT, DELETE, OPTIONS");
        return NoContent();
    }

    [HttpHead("by-id/{customerId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("HeadCustomerByIdV1")]
    [EndpointSummary("Check if the customer exists by id")]
    public async Task<IActionResult> HeadCustomer(int customerId, CancellationToken ct)
    {
        return await customerService.IsCustomerExistsByIdAsync(customerId, ct) ? Ok() : NotFound();
    }

    [HttpHead("by-email/{email}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("HeadCustomerByEmailV1")]
    [EndpointSummary("Check if the customer exists by email")]
    public async Task<IActionResult> HeadCustomerByEmail(string email, CancellationToken ct)
    {
        return await customerService.IsCustomerExistsByEmailAsync(email, ct) ? Ok() : NotFound();
    }

    [HttpHead("by-phone/{phone}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("HeadCustomerByPhoneV1")]
    [EndpointSummary("Check if the customer exists by phone")]
    public async Task<IActionResult> HeadCustomerByPhone(string phone, CancellationToken ct)
    {
        return await customerService.IsCustomerExistsByPhoneAsync(phone, ct) ? Ok() : NotFound();
    }

    [HttpGet("by-id/{customerId:int}", Name = "GetCustomerById")]
    [ProducesResponseType<CustomerDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetCustomerByIdV1")]
    [EndpointSummary("Retrieves a specific customer with its info")]
    [EndpointDescription("Retrieves a customer by ID and includes customer information.")]
    public async Task<ActionResult<CustomerDto>> GetCustomerById(int customerId, CancellationToken ct)
    {
        var customer = await customerService.GetCustomerByIdAsync(customerId, ct);
        return Ok(customer);
    }

    [HttpGet("by-email/{email}", Name = "GetCustomerByEmail")]
    [ProducesResponseType<CustomerDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetCustomerByEmailV1")]
    [EndpointSummary("Retrieves a specific customer with its info")]
    [EndpointDescription("Retrieves a customer by email and includes customer information.")]
    public async Task<ActionResult<CustomerDto>> GetCustomerByEmail(string email, CancellationToken ct)
    {
        var customerInfo = await customerService.GetCustomerByEmailAsync(email, ct);
        return Ok(customerInfo);
    }

    [HttpGet("by-phone/{phone}", Name = "GetCustomerByPhone")]
    [ProducesResponseType<CustomerDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetCustomerByPhoneV1")]
    [EndpointSummary("Retrieves a specific customer with its info")]
    [EndpointDescription("Retrieves a customer by phone and includes customer information.")]
    public async Task<ActionResult<CustomerDto>> GetCustomerByPhone(string phone, CancellationToken ct)
    {
        var customerInfo = await customerService.GetCustomerByPhoneAsync(phone, ct);
        return Ok(customerInfo);
    }

    [HttpGet]
    [ProducesResponseType<PagedResult<CustomerDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetPagedCustomersV1")]
    [EndpointSummary("Retrieves paged customers")]
    [EndpointDescription("Retrieves customers using pagination by query string (page, pageSize).")]
    public async Task<IActionResult> GetPagedCustomers(CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        int totalCount = await customerService.GetCustomersCountAsync(ct);
        var customers = await customerService.GetAllCustomersAsync(ct, page, pageSize);

        var pagedResult = PagedResult<CustomerDto>.Create(
            customers,
            totalCount,
            page,
            pageSize
        );

        return Ok(pagedResult);
    }

    [HttpPost]
    [Authorize(Roles = "System Administrator, Sales Representative")]
    [Consumes("application/json")]
    [ProducesResponseType<CreateCustomerDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("CreateCustomerV1")]
    [EndpointSummary("Creates a new customer")]
    [EndpointDescription("Registers a new customer into the system.")]
    public async Task<IActionResult> CreateCustomer(CreateCustomerDto customerDto, CancellationToken ct)
    {
        int newCustomerId = await customerService.AddNewCustomerAsync(customerDto, ct);

        return CreatedAtRoute(routeName: nameof(GetCustomerById),
            routeValues: new { Id = newCustomerId }, value: new { Id = newCustomerId });
    }

    [HttpPut("{customerId:int}")]
    [Authorize(Roles = "System Administrator, Sales Representative")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("UpdateCustomerV1")]
    [EndpointSummary("Update customer info")]
    [EndpointDescription("Update customer information.")]
    public async Task<IActionResult> UpdateCustomer(int customerId, CreateCustomerDto customerDto, CancellationToken ct)
    {
        await customerService.UpdateCustomerAsync(customerId, customerDto, ct);
        return NoContent();
    }

    [HttpDelete("{customerId:int}")]
    [Authorize(Roles = "System Administrator, Sales Representative")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("DeactivateCustomerV1")]
    [EndpointSummary("Deactivate customer by id")]
    [EndpointDescription("Deactivate the customer in the system.")]
    public async Task<IActionResult> DeactivateCustomer(int customerId, CancellationToken ct)
    {
        await customerService.DeactivateCustomerAsync(customerId, ct);
        return NoContent();
    }
}
