using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;

namespace WarehouseServices.Interfaces;

public interface IProductService
{
    Task<ProductDto?> GetProductByIdAsync(int productId, CancellationToken ct);
    Task<List<ProductDto>> GetAllProductsAsync(CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetProductsCountAsync(CancellationToken ct);
    Task<int> AddNewProductAsync(CreateProductDto product, CancellationToken ct);
    Task<bool> UpdateProductAsync(int productId, CreateProductDto product, CancellationToken ct);
    Task<bool> DeleteProductAsync(int productId, CancellationToken ct);
}
