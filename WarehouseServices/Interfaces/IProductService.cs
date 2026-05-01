using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;

namespace WarehouseServices.Interfaces;

public interface IProductService
{
    Task<ProductDto> GetProductByIdAsync(int productId);
    Task<List<ProductDto>> GetAllProductsAsync(int page = 1, int pageSize = 10);
    Task<int> GetProductsCountAsync();
    Task<int> AddNewProductAsync(CreateProductDto product);
    Task<bool> UpdateProductAsync(CreateProductDto product);
    Task<bool> DeleteProductAsync(int productId);
}
