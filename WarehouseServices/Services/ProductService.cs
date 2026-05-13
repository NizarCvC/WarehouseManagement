using Microsoft.Extensions.Logging;
using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;
using WarehouseDataAccess.Interfaces;
using WarehouseServices.Exceptions;
using WarehouseServices.Interfaces;

namespace WarehouseServices.Services;

public class ProductService(IProductRepository productRepository,
    ILogger<ProductService> logger) : IProductService
{
    public async Task<ProductDto> GetProductByIdAsync(int productId, CancellationToken ct)
    {
        Product? productInfo = await productRepository.GetProductByIdAsync(productId, ct);

        if (productInfo is null)
            throw new NotFoundException($"The product ID: {productId} not exists.");

        logger.LogInformation("The product id '{ProductId}' is retrieved", productId);
        return ProductDto.FromEntity(productInfo);
    }

    public async Task<ProductDto> GetProductBySkuAsync(string sku, CancellationToken ct)
    {
        Product? productInfo = await productRepository.GetProductBySkuAsync(sku, ct);

        if (productInfo is null)
            throw new NotFoundException($"The product SKU: {sku} not exists.");

        logger.LogInformation("The product sku '{Sku}' is retrieved", sku);
        return ProductDto.FromEntity(productInfo);
    }

    public async Task<ProductDto> GetProductByBarcodeAsync(string barcode, CancellationToken ct)
    {
        Product? productInfo = await productRepository.GetProductByBarcodeAsync(barcode, ct);

        if (productInfo is null)
            throw new NotFoundException($"The product barcode: {barcode} not exists.");

        logger.LogInformation("The product barcode '{Barcode}' is retrieved", barcode);
        return ProductDto.FromEntity(productInfo);
    }

    public async Task<List<ProductDto>> GetAllProductsAsync(CancellationToken ct, int page = 1, int pageSize = 10)
    {
        List<Product> products = await productRepository.GetAllProductsAsync(ct, page, pageSize);
        logger.LogInformation("The products retrieved in page {Page} with page size {PageSize}", page, pageSize);
        return ProductDto.FromEntities(products);
    }

    public async Task<int> GetProductsCountAsync(CancellationToken ct)
    {
        int count = await productRepository.GetProductsCountAsync(ct);
        logger.LogInformation("The products count retrieved");
        return count;
    }

    public async Task<int> AddNewProductAsync(CreateProductDto productDto, CancellationToken ct)
    {
        if (await productRepository.IsProductExistsBySkuAsync(productDto.Sku, ct))
            throw new ConflictException($"The product SKU {productDto.Sku} is already used.");


        if (await productRepository.IsProductExistsByBarcodeAsync(productDto.Barcode, ct))
            throw new ConflictException($"The product barcode {productDto.Barcode} is already used.");

        int newId = await productRepository.AddNewProductAsync(productDto, ct);

        if (newId == -1)
        {
            logger.LogError("Failed to add the product with SKU '{Sku}' in the system.", productDto.Sku);
            throw new InternalServerErrorException("Failed to add the product.");
        }

        logger.LogInformation("A new product with SKU '{Sku}' was added successfully with ID {NewId}", productDto.Sku, newId);
        return newId;
    }

    public async Task UpdateProductAsync(int productId, CreateProductDto productDto, CancellationToken ct)
    {
        Product? productInfo = await productRepository.GetProductByIdAsync(productId, ct);

        if (productInfo is null)
            throw new NotFoundException($"The product ID: {productId} not exists.");

        if (productInfo.Sku != productDto.Sku)
        {
            bool isSkuUsed = await productRepository.IsProductExistsBySkuAsync(productDto.Sku, ct);

            if (isSkuUsed)
                throw new ConflictException($"The product SKU {productDto.Sku} is already used.");
        }

        if (productInfo.Barcode != productDto.Barcode)
        {
            bool isBarcodeUsed = await productRepository.IsProductExistsByBarcodeAsync(productDto.Barcode, ct);

            if (isBarcodeUsed)
                throw new ConflictException($"The product barcode {productDto.Barcode} is already used.");
        }

        bool isSuccess = await productRepository.UpdateProductAsync(productId, productDto, ct);

        if (!isSuccess)
        {
            logger.LogError("Failed to update the product with id '{ProductId}' in the system.", productId);
            throw new InternalServerErrorException("Failed to update the product.");
        }

        logger.LogInformation("The product with ID '{ProductId}' was updated successfully", productId);
    }

    public async Task DeactivateProductAsync(int productId, CancellationToken ct)
    {
        bool isSuccess = await productRepository.DeactivateProductAsync(productId, ct);

        if (!isSuccess)
            throw new NotFoundException($"The product with ID: {productId} not exists.");

        logger.LogInformation("The product with ID '{ProductId}' was deactivated", productId);
    }

    public async Task<bool> IsProductExistsByIdAsync(int productId, CancellationToken ct)
    {
        bool isFound = await productRepository.IsProductExistsByIdAsync(productId, ct);

        if (isFound)
            logger.LogDebug("The product with id '{ProductId}' is found.", productId);
        else
            logger.LogDebug("The product with id '{ProductId}' is not found.", productId);

        return isFound;
    }

    public async Task<bool> IsProductExistsBySkuAsync(string sku, CancellationToken ct)
    {
        bool isFound = await productRepository.IsProductExistsBySkuAsync(sku, ct);

        if (isFound)
            logger.LogDebug("The product with sku '{Sku}' is found.", sku);
        else
            logger.LogDebug("The product with sku '{Sku}' is not found.", sku);

        return isFound;
    }

    public async Task<bool> IsProductExistsByBarcodeAsync(string barcode, CancellationToken ct)
    {
        bool isFound = await productRepository.IsProductExistsByBarcodeAsync(barcode, ct);

        if (isFound)
            logger.LogDebug("The product with barcode '{Barcode}' is found.", barcode);
        else
            logger.LogDebug("The product with barcode '{Barcode}' is not found.", barcode);

        return isFound;
    }
}
