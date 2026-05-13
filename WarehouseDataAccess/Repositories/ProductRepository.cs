using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;
using WarehouseCore.enums;
using WarehouseDataAccess.Interfaces;

namespace WarehouseDataAccess.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<Product?> GetProductByIdAsync(int productId, CancellationToken ct)
    {
        string query = @"SELECT p.ProductID, p.Name AS ProductName, p.Sku, p.Barcode, p.Description, p.PurchasePrice,
                        p.SalePrice, p.MinStock, p.IsActive, p.CreatedAt AS ProductCreatedAt, p.UpdatedAt, p.UnitID,
                        p.CategoryID, c.Name AS CategoryName, c.CreatedAt AS CategoryCreatedAt, c.ParentID
                        FROM Products p LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                        WHERE P.ProductID = @ProductID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@ProductID", System.Data.SqlDbType.Int) { Value = productId });
            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                if (await reader.ReadAsync(ct))
                    return MapReaderToProduct(reader);
                else
                    return null;
            }
        }
    }

    public async Task<Product?> GetProductBySkuAsync(string sku, CancellationToken ct)
    {
        string query = @"SELECT p.ProductID, p.Name AS ProductName, p.Sku, p.Barcode, p.Description, p.PurchasePrice,
                        p.SalePrice, p.MinStock, p.IsActive, p.CreatedAt AS ProductCreatedAt, p.UpdatedAt, p.UnitID,
                        p.CategoryID, c.Name AS CategoryName, c.CreatedAt AS CategoryCreatedAt, c.ParentID
                        FROM Products p LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                        WHERE P.Sku = @Sku";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@Sku", SqlDbType.VarChar) { Value = sku });
            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                if (await reader.ReadAsync(ct))
                    return MapReaderToProduct(reader);
                else
                    return null;
            }
        }
    }

    public async Task<Product?> GetProductByBarcodeAsync(string barcode, CancellationToken ct)
    {
        string query = @"SELECT p.ProductID, p.Name AS ProductName, p.Sku, p.Barcode, p.Description, p.PurchasePrice,
                        p.SalePrice, p.MinStock, p.IsActive, p.CreatedAt AS ProductCreatedAt, p.UpdatedAt, p.UnitID,
                        p.CategoryID, c.Name AS CategoryName, c.CreatedAt AS CategoryCreatedAt, c.ParentID
                        FROM Products p LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                        WHERE P.Barcode = @Barcode";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@Barcode", SqlDbType.VarChar) { Value = barcode });
            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                if (await reader.ReadAsync(ct))
                    return MapReaderToProduct(reader);
                else
                    return null;
            }
        }
    }

    public async Task<List<Product>> GetAllProductsAsync(CancellationToken ct, int page = 1, int pageSize = 10)
    {
        List<Product> products = new List<Product>();

        string query = @"SELECT p.ProductID, p.Name AS ProductName, p.Sku, p.Barcode, p.Description, p.PurchasePrice,
                        p.SalePrice, p.MinStock, p.IsActive, p.CreatedAt AS ProductCreatedAt, p.UpdatedAt, p.UnitID,
                        p.CategoryID, c.Name AS CategoryName, c.CreatedAt AS CategoryCreatedAt, c.ParentID
                        FROM Products p LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                        WHERE p.IsActive = 1
                        ORDER BY p.ProductID
                        OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
                        FETCH NEXT @RowsPerPage ROWS ONLY";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@PageNumber", System.Data.SqlDbType.Int) { Value = page });
            command.Parameters.Add(new SqlParameter("@RowsPerPage", System.Data.SqlDbType.Int) { Value = pageSize });
            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    Product product = MapReaderToProduct(reader); // تصحيح اسم المتغير
                    products.Add(product);
                }

                return products;
            }
        }
    }

    public async Task<int> GetProductsCountAsync(CancellationToken ct)
    {
        int countNumber = 0;
        string query = @"SELECT COUNT(*) FROM Products";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            await connection.OpenAsync(ct);

            object result = await command.ExecuteScalarAsync(ct);

            if (result != null && result != DBNull.Value)
                countNumber = Convert.ToInt32(result);

            return countNumber;
        }
    }

    public async Task<int> AddNewProductAsync(CreateProductDto product, CancellationToken ct)
    {
        int newProductId = -1;

        string query = @"INSERT INTO Products (Name, Sku, Barcode, Description, PurchasePrice, SalePrice, MinStock, UnitID, CategoryID)
                    OUTPUT INSERTED.ProductID
                    VALUES (@Name, @Sku, @Barcode, @Description, @PurchasePrice, @SalePrice, @MinStock, @UnitID, @CategoryID)";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar) { Value = product.Name });
            command.Parameters.Add(new SqlParameter("@Sku", SqlDbType.VarChar) { Value = product.Sku });
            command.Parameters.Add(new SqlParameter("@Barcode", SqlDbType.VarChar) { Value = product.Barcode });
            command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar) { Value = (object?)product.Description ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@PurchasePrice", SqlDbType.Decimal) { Value = product.PurchasePrice });
            command.Parameters.Add(new SqlParameter("@SalePrice", SqlDbType.Decimal) { Value = product.SalePrice });
            command.Parameters.Add(new SqlParameter("@MinStock", SqlDbType.Int) { Value = product.MinStock });
            command.Parameters.Add(new SqlParameter("@UnitID", SqlDbType.Int) { Value = product.UnitID });
            command.Parameters.Add(new SqlParameter("@CategoryID", SqlDbType.Int) { Value = (object?)product.CategoryID ?? DBNull.Value });
            await connection.OpenAsync(ct);

            object result = await command.ExecuteScalarAsync(ct);
            if (result != null && result != DBNull.Value)
                newProductId = Convert.ToInt32(result);

            return newProductId;
        }
    }

    public async Task<bool> UpdateProductAsync(int productId, CreateProductDto product, CancellationToken ct)
    {
        string query = @"UPDATE Products
                        SET Name = @Name, 
                        Sku = @Sku, 
                        Barcode = @Barcode, 
                        Description = @Description,
                        PurchasePrice = @PurchasePrice, 
                        SalePrice = @SalePrice, 
                        MinStock = @MinStock,
                        UnitID = @UnitID, 
                        CategoryID = @CategoryID, 
                        UpdatedAt = GETDATE()
                        WHERE ProductID = @ProductID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@ProductID", SqlDbType.Int) { Value = productId });
            command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar) { Value = product.Name });
            command.Parameters.Add(new SqlParameter("@Sku", SqlDbType.VarChar) { Value = product.Sku });
            command.Parameters.Add(new SqlParameter("@Barcode", SqlDbType.VarChar) { Value = product.Barcode });
            command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar) { Value = (object?)product.Description ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@PurchasePrice", SqlDbType.Decimal) { Value = product.PurchasePrice });
            command.Parameters.Add(new SqlParameter("@SalePrice", SqlDbType.Decimal) { Value = product.SalePrice });
            command.Parameters.Add(new SqlParameter("@MinStock", SqlDbType.Int) { Value = product.MinStock });
            command.Parameters.Add(new SqlParameter("@UnitID", SqlDbType.Int) { Value = product.UnitID });
            command.Parameters.Add(new SqlParameter("@CategoryID", SqlDbType.Int) { Value = (object?)product.CategoryID ?? DBNull.Value });
            await connection.OpenAsync(ct);

            int rowsAffected = await command.ExecuteNonQueryAsync(ct);

            return rowsAffected > 0;
        }
    }

    public async Task<bool> DeactivateProductAsync(int productId, CancellationToken ct)
    {
        string query = @"UPDATE Products SET IsActive = 0 WHERE ProductID = @ProductID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@ProductID", SqlDbType.Int) { Value = productId });
            await connection.OpenAsync(ct);

            int rowsAffected = await command.ExecuteNonQueryAsync(ct);

            return rowsAffected > 0;
        }
    }

    private Product MapReaderToProduct(SqlDataReader reader)
    {
        return new Product()
        {
            ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
            Name = reader.GetString(reader.GetOrdinal("ProductName")),
            Sku = reader.GetString(reader.GetOrdinal("Sku")),
            Barcode = reader.GetString(reader.GetOrdinal("Barcode")),
            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ?
                null : reader.GetString(reader.GetOrdinal("Description")),
            PurchasePrice = reader.GetDecimal(reader.GetOrdinal("PurchasePrice")),
            SalePrice = reader.GetDecimal(reader.GetOrdinal("SalePrice")),
            MinStock = reader.GetInt32(reader.GetOrdinal("MinStock")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("ProductCreatedAt")),
            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ?
                null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
            Unit = (enUnit)reader.GetInt32(reader.GetOrdinal("UnitID")),
            CategoryID = reader.IsDBNull(reader.GetOrdinal("CategoryID")) ?
                 null : reader.GetInt32(reader.GetOrdinal("CategoryID")),
            Category = reader.IsDBNull(reader.GetOrdinal("CategoryID")) ? null : new Category()
            {
                CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                Name = reader.GetString(reader.GetOrdinal("CategoryName")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CategoryCreatedAt")),
            }
        };
    }

    public async Task<bool> IsProductExistsByIdAsync(int productId, CancellationToken ct)
    {
       string query = @"SELECT 1 FROM Products WHERE ProductID = @ProductID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@ProductID", SqlDbType.Int) { Value = productId });
            await connection.OpenAsync(ct);

            object? result = await command.ExecuteScalarAsync(ct);
            return result != null;
        }
    }

    public async Task<bool> IsProductExistsBySkuAsync(string sku, CancellationToken ct)
    {
        string query = @"SELECT 1 FROM Products WHERE Sku = @Sku";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@Sku", SqlDbType.VarChar) { Value = sku });
            await connection.OpenAsync(ct);

            object? result = await command.ExecuteScalarAsync(ct);
            return result != null;
        }
    }

    public async Task<bool> IsProductExistsByBarcodeAsync(string barcode, CancellationToken ct)
    {
        string query = @"SELECT 1 FROM Products WHERE Barcode = @Barcode";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@Barcode", SqlDbType.VarChar) { Value = barcode });
            await connection.OpenAsync(ct);

            object? result = await command.ExecuteScalarAsync(ct);
            return result != null;
        }
    }
}