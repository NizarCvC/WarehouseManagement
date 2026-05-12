using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;

namespace WarehouseServices.Interfaces;

public interface IProductService
{
    Task<ProductDto?> GetProductByIdAsync(int productId, CancellationToken ct);
    Task<List<ProductDto>> GetAllProductsAsync(CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetProductsCountAsync(CancellationToken ct);
    Task<int> AddNewProductAsync(CreateProductDto productDto, CancellationToken ct);
    Task UpdateProductAsync(int productId, CreateProductDto productDto, CancellationToken ct);
    Task DeactivateProductAsync(int productId, CancellationToken ct);
}
