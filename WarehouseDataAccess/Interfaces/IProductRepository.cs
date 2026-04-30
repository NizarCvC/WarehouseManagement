using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;

namespace WarehouseDataAccess.Interfaces;

public interface IProductRepository
{   
    public Task<Product> GetProductByIdAsync(int productId);
    public Task<List<Product>> GetAllProductsAsync(int page = 1, int pageSize = 10);
    public Task<int> GetProductsCountAsync();
    public Task<int> AddNewProductAsync(CreateProductDto product);
    public Task<bool> UpdateProductAsync(CreateProductDto product);
    public Task<bool> DeleteProductAsync(int productId);
}
