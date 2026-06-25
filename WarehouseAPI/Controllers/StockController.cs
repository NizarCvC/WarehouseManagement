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
[Route("api/stocks")]
[Authorize]
[Tags("Stocks")]
[Produces("application/json")]
[EnableRateLimiting(policyName: "GeneralPolicy")]
public class StockController(IStockService stockService) : ControllerBase
{
    [HttpOptions]
    [AllowAnonymous] 
    [DisableRateLimiting]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [EndpointName("StockOptionsV1")]
    [EndpointSummary("Get the available options in Stocks endpoints.")]
    public IActionResult StockOptions()
    {
        Response.Headers.Append("Allow", "GET, HEAD, POST, PUT, OPTIONS");
        return NoContent();
    }

    [HttpPost("transfer")]
    [Authorize(Roles = "System Administrator,Warehouse Manager")] 
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("CreateStockTransferV1")]
    [EndpointSummary("Creates a new stock transfer.")]
    [EndpointDescription("Creates a new stock transfer with the provided details.")]
    public async Task<IActionResult> CreateStockTransfer(CreateStockTransferDto transfer, CancellationToken ct)
    {
        int newTransferId = await stockService.CreateStockTransferAsync(transfer, ct);
        
        return Created(string.Empty, new { TransferId = newTransferId });
    }

    [HttpPut("transfer/{transferId:int}/status/{newStatusId:int}")]
    [Authorize(Roles = "System Administrator,Warehouse Manager")] 
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("UpdateTransferStatusV1")]
    [EndpointSummary("Updates the status of a stock transfer.")]
    [EndpointDescription("Updates the status of a stock transfer with the provided transfer ID and new status ID.")]
    public async Task<IActionResult> UpdateTransferStatus(int transferId, int newStatusId, CancellationToken ct)
    {
        await stockService.UpdateTransferStatusAsync(transferId, newStatusId, ct);
        return NoContent();
    }

    [HttpGet("products/{productId:int}/warehouses/{warehouseId:int}")]
    [Authorize(Roles = "System Administrator,Accountant,Warehouse Manager,Sales Representative,Purchasing Officer")] // مسموح للجميع
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetProductStockInWarehouseV1")]
    [EndpointSummary("Retrieves the stock of a product in a specific warehouse.")]
    [EndpointDescription("Retrieves the stock information of a product in a specific warehouse.")]
    public async Task<ActionResult<CurrentStockDto>> GetProductStockInWarehouse(int productId, int warehouseId, CancellationToken ct)
    {
        CurrentStockDto productStock = await stockService.GetProductStockInWarehouseAsync(productId, warehouseId, ct);
        return Ok(productStock);
    }

    [HttpGet("products/{productId:int}")]
    [Authorize(Roles = "System Administrator,Accountant,Warehouse Manager,Sales Representative,Purchasing Officer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetProductStockV1")]
    [EndpointSummary("Retrieves the stock of a product across all warehouses.")]
    [EndpointDescription("Retrieves the stock information of a product across all warehouses.")]
    public async Task<IActionResult> GetProductStock(int productId, CancellationToken ct)
    {
        List<CurrentStockDto> productStock = await stockService.GetProductStockAsync(productId, ct);
        return Ok(productStock);
    }

    [HttpGet("warehouses/{warehouseId:int}")]
    [Authorize(Roles = "System Administrator,Accountant,Warehouse Manager,Sales Representative,Purchasing Officer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetWarehouseStockV1")]
    [EndpointSummary("Retrieves the stock of a warehouse.")]
    [EndpointDescription("Retrieves the stock information of a warehouse.")]
    public async Task<IActionResult> GetWarehouseStock(int warehouseId, CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        int totalCount = await stockService.GetWarehouseStockCountAsync(warehouseId, ct);
        var warehouseStock = await stockService.GetWarehouseStockAsync(warehouseId, ct, page, pageSize);

        var pagedResult = PagedResult<CurrentStockDto>.Create(
            warehouseStock,
            totalCount,
            page,
            pageSize
        );

        return Ok(pagedResult);
    }

    [HttpGet("ledger")]
    [Authorize(Roles = "System Administrator,Accountant,Warehouse Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetItemLedgerV1")]
    [EndpointSummary("Retrieves the item ledger.")]
    [EndpointDescription("Retrieves the item ledger with optional filtering by product ID and warehouse ID.")]
    public async Task<IActionResult> GetItemLedger(CancellationToken ct, [FromQuery] int? productId,
        [FromQuery] int? warehouseId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        int totalCount = await stockService.GetItemLedgerCountAsync(productId, warehouseId, ct);
        var itemLedger = await stockService.GetItemLedgerAsync(productId, warehouseId, ct, page, pageSize);

        var pagedResult = PagedResult<ItemLedgerDto>.Create(
            itemLedger,
            totalCount,
            page,
            pageSize
        );

        return Ok(pagedResult);
    }
}