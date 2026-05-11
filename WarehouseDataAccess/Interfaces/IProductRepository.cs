using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;

namespace WarehouseDataAccess.Interfaces;

public interface IProductRepository
{   
    Task<Product?> GetProductByIdAsync(int productId, CancellationToken ct);
    Task<List<Product>> GetAllProductsAsync(CancellationToken ct, int page = 1, int pageSize = 10);
    Task<int> GetProductsCountAsync(CancellationToken ct);
    Task<int> AddNewProductAsync(CreateProductDto product, CancellationToken ct);
    Task<bool> UpdateProductAsync(int productId, CreateProductDto product, CancellationToken ct);
    Task<bool> DeactivateProductAsync(int productId, CancellationToken ct);
}
