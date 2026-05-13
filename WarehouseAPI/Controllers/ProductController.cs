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
[Route("api/products")]
[Authorize(Roles = "System Administrator, Purchasing Officer, Warehouse Manager, Sales Representative, Accountant")]
[Tags("Products")]
[Produces("application/json")]
[EnableRateLimiting(policyName: "GeneralPolicy")]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpOptions]
    [DisableRateLimiting]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [EndpointName("ProductOptionsV1")]
    [EndpointSummary("Get the available options in Products endpoints.")]
    public IActionResult ProductOptions()
    {
        Response.Headers.Append("Allow", "GET, HEAD, POST, PUT, DELETE, OPTIONS");
        return NoContent();
    }

    [HttpHead("by-id/{productId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("HeadProductByIdV1")]
    [EndpointSummary("Check if the product exists by id")]
    public async Task<IActionResult> HeadProduct(int productId, CancellationToken ct)
    {
        return await productService.IsProductExistsByIdAsync(productId, ct) ? Ok() : NotFound();
    }

    [HttpHead("by-sku/{sku}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("HeadProductBySkuV1")]
    [EndpointSummary("Check if the product exists by SKU")]
    public async Task<IActionResult> HeadProductBySku(string sku, CancellationToken ct)
    {
        return await productService.IsProductExistsBySkuAsync(sku, ct) ? Ok() : NotFound();
    }

    [HttpHead("by-barcode/{barcode}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("HeadProductByBarcodeV1")]
    [EndpointSummary("Check if the product exists by barcode")]
    public async Task<IActionResult> HeadProductByBarcode(string barcode, CancellationToken ct)
    {
        return await productService.IsProductExistsByBarcodeAsync(barcode, ct) ? Ok() : NotFound();
    }

    [HttpGet("by-id/{productId:int}", Name = "GetProductById")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetProductByIdV1")]
    [EndpointSummary("Retrieves a specific product with its info")]
    [EndpointDescription("Retrieves a product by ID and includes product information.")]
    public async Task<ActionResult<ProductDto>> GetProductById(int productId, CancellationToken ct)
    {
        var productInfo = await productService.GetProductByIdAsync(productId, ct);
        return Ok(productInfo);
    }

    [HttpGet("by-sku/{sku}", Name = "GetProductBySku")]
    [ProducesResponseType<ProductDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetProductBySkuV1")]
    [EndpointSummary("Retrieves a specific product with its info")]
    [EndpointDescription("Retrieves a product by SKU and includes product information.")]
    public async Task<ActionResult<ProductDto>> GetProductBySku(string sku, CancellationToken ct)
    {
        var productInfo = await productService.GetProductBySkuAsync(sku, ct);
        return Ok(productInfo);
    }

    [HttpGet("by-barcode/{barcode}", Name = "GetProductByBarcode")]
    [ProducesResponseType<ProductDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetProductByBarcodeV1")]
    [EndpointSummary("Retrieves a specific product with its info")]
    [EndpointDescription("Retrieves a product by barcode and includes product information.")]
    public async Task<ActionResult<ProductDto>> GetProductByBarcode(string barcode, CancellationToken ct)
    {
        var productInfo = await productService.GetProductByBarcodeAsync(barcode, ct);
        return Ok(productInfo);
    }

    [HttpGet]
    [ProducesResponseType<PagedResult<ProductDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetPagedProductsV1")]
    [EndpointSummary("Retrieves paged products")]
    [EndpointDescription("Retrieves products using pagination by query string (page, pageSize).")]
    public async Task<IActionResult> GetPagedProducts(CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        int totalCount = await productService.GetProductsCountAsync(ct);
        var products = await productService.GetAllProductsAsync(ct, page, pageSize);

        var pagedResult = PagedResult<ProductDto>.Create(
            products,
            totalCount,
            page,
            pageSize
        );

        return Ok(pagedResult);
    }

    [HttpPost]
    [Authorize(Roles = "System Administrator, Purchasing Officer")]
    [Consumes("application/json")]
    [ProducesResponseType<CreateProductDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("CreateProductV1")]
    [EndpointSummary("Creates a new product")]
    [EndpointDescription("Registers a new product into the system.")]
    public async Task<IActionResult> CreateProduct(CreateProductDto productDto, CancellationToken ct)
    {
        int newProductId = await productService.AddNewProductAsync(productDto, ct);

        return CreatedAtRoute(routeName: nameof(GetProductById),
            routeValues: new { Id = newProductId }, value: new { Id = newProductId });
    }

    [HttpPut("{productId:int}")]
    [Consumes("application/json")]
    [Authorize(Roles = "System Administrator, Purchasing Officer, Warehouse Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("UpdateProductV1")]
    [EndpointSummary("Update product info")]
    [EndpointDescription("Update product information.")]
    public async Task<IActionResult> UpdateProduct(int productId, CreateProductDto productDto, CancellationToken ct)
    {
        await productService.UpdateProductAsync(productId, productDto, ct);
        return NoContent();
    }

    [HttpDelete("{productId:int}")]
    [Authorize(Roles = "System Administrator, Purchasing Officer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("DeactivateProductV1")]
    [EndpointSummary("Deactivate product by id")]
    [EndpointDescription("Deactivate the product in the system.")]
    public async Task<IActionResult> DeactivateProduct(int productId, CancellationToken ct)
    {
        await productService.DeactivateProductAsync(productId, ct);
        return NoContent();
    }
}
