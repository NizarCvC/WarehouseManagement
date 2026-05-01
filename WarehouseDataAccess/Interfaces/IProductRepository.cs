using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;

namespace WarehouseDataAccess.Interfaces;

public interface IProductRepository
{   
    Task<Product?> GetProductByIdAsync(int productId);
    Task<List<Product>> GetAllProductsAsync(int page = 1, int pageSize = 10);
    Task<int> GetProductsCountAsync();
    Task<int> AddNewProductAsync(CreateProductDto product);
    Task<bool> UpdateProductAsync(int productId, CreateProductDto product);
    Task<bool> DeleteProductAsync(int productId);
}
