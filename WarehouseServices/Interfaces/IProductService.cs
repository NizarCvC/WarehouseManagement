using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;

namespace WarehouseServices.Interfaces;

public interface IProductService
{
    Task<ProductDto> GetProductByIdAsync(int productId, CancellationToken ct);
    Task<ProductDto> GetProductBySkuAsync(string sku, CancellationToken ct);
    Task<ProductDto> GetProductByBarcodeAsync(string barcode, CancellationToken ct);
    Task<List<ProductDto>> GetAllProductsAsync(CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetProductsCountAsync(CancellationToken ct);
    Task<int> AddNewProductAsync(CreateProductDto productDto, CancellationToken ct);
    Task UpdateProductAsync(int productId, CreateProductDto productDto, CancellationToken ct);
    Task DeactivateProductAsync(int productId, CancellationToken ct);
    Task<bool> IsProductExistsByIdAsync(int productId, CancellationToken ct);
    Task<bool> IsProductExistsBySkuAsync(string sku, CancellationToken ct);
    Task<bool> IsProductExistsByBarcodeAsync(string barcode, CancellationToken ct);
}
