using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;
using WarehouseDataAccess.Interfaces;

namespace WarehouseDataAccess.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly string _connectionString;

    public SupplierRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<Supplier?> GetSupplierByIdAsync(int supplierId, CancellationToken ct)
    {
        string query = @"SELECT s.SupplierID, s.Name, s.Phone, s.Email, s.Address, s.TaxNumber, s.IsActive, s.CreatedAt 
                        FROM Suppliers s 
                        WHERE s.SupplierID = @SupplierID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@SupplierID", SqlDbType.Int) { Value = supplierId });
            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                if (await reader.ReadAsync(ct))
                    return MapReaderToSupplier(reader);
                else
                    return null;
            }
        }
    }

    public async Task<Supplier?> GetSupplierByEmailAsync(string email, CancellationToken ct)
    {
        string query = @"SELECT s.SupplierID, s.Name, s.Phone, s.Email, s.Address, s.TaxNumber, s.IsActive, s.CreatedAt 
                        FROM Suppliers s 
                        WHERE s.Email = @Email";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar) { Value = email });
            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                if (await reader.ReadAsync(ct))
                    return MapReaderToSupplier(reader);
                else
                    return null;
            }
        }
    }

    public async Task<Supplier?> GetSupplierByPhoneAsync(string phone, CancellationToken ct)
    {
        string query = @"SELECT s.SupplierID, s.Name, s.Phone, s.Email, s.Address, s.TaxNumber, s.IsActive, s.CreatedAt 
                        FROM Suppliers s 
                        WHERE s.Phone = @Phone";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@Phone", SqlDbType.VarChar) { Value = phone });
            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                if (await reader.ReadAsync(ct))
                    return MapReaderToSupplier(reader);
                else
                    return null;
            }
        }
    }

    public async Task<Supplier?> GetSupplierByTaxNumberAsync(string taxNumber, CancellationToken ct)
    {
        string query = @"SELECT s.SupplierID, s.Name, s.Phone, s.Email, s.Address, s.TaxNumber, s.IsActive, s.CreatedAt 
                        FROM Suppliers s 
                        WHERE s.TaxNumber = @TaxNumber";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@TaxNumber", SqlDbType.VarChar) { Value = taxNumber });
            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                if (await reader.ReadAsync(ct))
                    return MapReaderToSupplier(reader);
                else
                    return null;
            }
        }
    }

    public async Task<List<Supplier>> GetAllSuppliersAsync(CancellationToken ct, int page = 1, int pageSize = 10)
    {
        List<Supplier> suppliers = new List<Supplier>();
        string query = @"SELECT s.SupplierID, s.Name, s.Phone, s.Email, s.Address, s.TaxNumber, s.IsActive, s.CreatedAt 
                        FROM Suppliers s
                        WHERE s.IsActive = 1
                        ORDER BY s.SupplierID
                        OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
                        FETCH NEXT @RowsPerPage ROWS ONLY";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@PageNumber", SqlDbType.Int) { Value = page });
            command.Parameters.Add(new SqlParameter("@RowsPerPage", SqlDbType.Int) { Value = pageSize });
            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    Supplier supplier = MapReaderToSupplier(reader);
                    suppliers.Add(supplier);
                }

                return suppliers;
            }
        }
    }

    public async Task<int> GetSuppliersCountAsync(CancellationToken ct)
    {
        int countNumber = 0;
        string query = @"SELECT COUNT(*) FROM Suppliers";

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

    public async Task<int> AddNewSupplierAsync(CreateSupplierDto supplier, CancellationToken ct)
    {
        int newSupplierId = -1;
        string query = @"INSERT INTO Suppliers (Name, Phone, Email, Address, TaxNumber)
                        VALUES (@Name, @Phone, @Email, @Address, @TaxNumber)
                        SELECT SCOPE_IDENTITY()";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar) { Value = supplier.Name });
            command.Parameters.Add(new SqlParameter("@Phone", SqlDbType.VarChar) { Value = supplier.Phone });
            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar) { Value = supplier.Email });
            command.Parameters.Add(new SqlParameter("@Address", SqlDbType.NVarChar) { Value = supplier.Address });
            command.Parameters.Add(new SqlParameter("@TaxNumber", SqlDbType.VarChar) { Value = supplier.TaxNumber });

            await connection.OpenAsync(ct);

            object result = await command.ExecuteScalarAsync(ct);
            if (result != null && result != DBNull.Value)
                newSupplierId = Convert.ToInt32(result);

            return newSupplierId;
        }
    }

    public async Task<bool> UpdateSupplierAsync(int supplierId, CreateSupplierDto supplier, CancellationToken ct)
    {
        string query = @"UPDATE Suppliers 
                        SET Name = @Name,
                            Phone = @Phone,
                            Email = @Email,
                            Address = @Address,
                            TaxNumber = @TaxNumber
                        WHERE SupplierID = @SupplierID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@SupplierID", SqlDbType.Int) { Value = supplierId });
            command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar) { Value = supplier.Name });
            command.Parameters.Add(new SqlParameter("@Phone", SqlDbType.VarChar) { Value = supplier.Phone });
            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar) { Value = supplier.Email });
            command.Parameters.Add(new SqlParameter("@Address", SqlDbType.NVarChar) { Value = supplier.Address });
            command.Parameters.Add(new SqlParameter("@TaxNumber", SqlDbType.VarChar) { Value = supplier.TaxNumber });

            await connection.OpenAsync(ct);

            int rowsAffected = await command.ExecuteNonQueryAsync(ct);
            return rowsAffected > 0;
        }
    }

    public async Task<bool> DeactivateSupplierAsync(int supplierId, CancellationToken ct)
    {
        string query = @"UPDATE Suppliers SET IsActive = 0 WHERE SupplierID = @SupplierID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@SupplierID", SqlDbType.Int) { Value = supplierId });
            await connection.OpenAsync(ct);

            int rowsAffected = await command.ExecuteNonQueryAsync(ct);
            return rowsAffected > 0;
        }
    }

    public async Task<bool> IsSupplierExistsByIdAsync(int supplierId, CancellationToken ct)
    {
        string query = @"SELECT 1 FROM Suppliers WHERE SupplierID = @SupplierID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@SupplierID", SqlDbType.Int) { Value = supplierId });
            await connection.OpenAsync(ct);

            object? result = await command.ExecuteScalarAsync(ct);
            return result != null;
        }
    }

    public async Task<bool> IsSupplierExistsByEmailAsync(string email, CancellationToken ct)
    {
        string query = @"SELECT 1 FROM Suppliers WHERE Email = @Email";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar) { Value = email });
            await connection.OpenAsync(ct);

            object? result = await command.ExecuteScalarAsync(ct);
            return result != null;
        }
    }   

    public async Task<bool> IsSupplierExistsByPhoneAsync(string phone, CancellationToken ct)
    {
        string query = @"SELECT 1 FROM Suppliers WHERE Phone = @Phone";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@Phone", SqlDbType.VarChar) { Value = phone });
            await connection.OpenAsync(ct);

            object? result = await command.ExecuteScalarAsync(ct);
            return result != null;
        }
    }

    public async Task<bool> IsSupplierExistsByTaxNumberAsync(string taxNumber, CancellationToken ct)
    {
        string query = @"SELECT 1 FROM Suppliers WHERE TaxNumber = @TaxNumber";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@TaxNumber", SqlDbType.VarChar) { Value = taxNumber });
            await connection.OpenAsync(ct);

            object? result = await command.ExecuteScalarAsync(ct);
            return result != null;
        }
    }

    private Supplier MapReaderToSupplier(SqlDataReader reader)
    {
        return new Supplier()
        {
            SupplierID = reader.GetInt32(reader.GetOrdinal("SupplierID")),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            Phone = reader.GetString(reader.GetOrdinal("Phone")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            Address = reader.GetString(reader.GetOrdinal("Address")),
            TaxNumber = reader.GetString(reader.GetOrdinal("TaxNumber")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
        };
    }
}