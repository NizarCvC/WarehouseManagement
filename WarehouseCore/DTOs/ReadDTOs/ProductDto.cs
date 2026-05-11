using WarehouseCore.Entities;

namespace WarehouseCore.DTOs.ReadDTOs;

public class ProductDto
{
    public int ProductID { get; set; }
    public required string Name { get; set; }
    public required string Sku { get; set; }
    public required string Barcode { get; set; }
    public string? Description { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public int MinStock { get; set; }
    public bool IsActive { get; set; }
    public string? CategoryName { get; set; }
    public required string UnitName { get; set; }

    public static ProductDto FromEntity(Product product)
    {
        return new ProductDto()
        {
            ProductID = product.ProductID,
            Name = product.Name,
            Sku = product.Sku,
            Barcode = product.Barcode,
            Description = product.Description,
            PurchasePrice = product.PurchasePrice,
            SalePrice = product.SalePrice,
            MinStock = product.MinStock,
            IsActive = product.IsActive,
            CategoryName = product.Category?.Name,
            UnitName = product.Unit.ToString()
        };
    }

    public static List<ProductDto> FromEntities(List<Product> products)
    {
        return products.Select(FromEntity).ToList();
    }
}
