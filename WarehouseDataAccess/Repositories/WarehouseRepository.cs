using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;
using WarehouseDataAccess.Interfaces;

namespace WarehouseDataAccess.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly string _connectionString;

    public WarehouseRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<Warehouse?> GetWarehouseByIdAsync(int warehouseId, CancellationToken ct)
    {
        string query = @"SELECT w.WarehouseID, w.Name, w.Code, w.[Location], w.IsActive, w.CreatedAt FROM Warehouses w
                        WHERE w.WarehouseID = @WarehouseID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@WarehouseID", System.Data.SqlDbType.Int) { Value = warehouseId });
            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                if (await reader.ReadAsync(ct))
                    return MapReaderToWarehouse(reader);
                else
                    return null;
            }
        }
    }

    public async Task<List<Warehouse>> GetAllWarehousesAsync(CancellationToken ct, int page = 1, int pageSize = 10)
    {
        List<Warehouse> warehouses = new List<Warehouse>();
        string query = @"SELECT w.WarehouseID, w.Name, w.Code, w.[Location], w.IsActive, w.CreatedAt FROM Warehouses w
                            WHERE w.IsActive = 1
                            ORDER BY w.WarehouseID
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
                    Warehouse warehouse = MapReaderToWarehouse(reader);
                    warehouses.Add(warehouse);
                }

                return warehouses;
            }
        }
    }

    public async Task<int> GetWarehousesCountAsync(CancellationToken ct)
    {
        int countNumber = 0;
        string query = @"SELECT COUNT(*) FROM Warehouses";

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

    public async Task<int> AddNewWarehouseAsync(CreateWarehouseDto warehouse, CancellationToken ct)
    {
        int newWarehouseId = -1;
        string query = @"INSERT INTO Warehouses (Name, Code, [Location])
                        VALUES (@Name, @Code, @Location)
                        SELECT SCOPE_IDENTITY()";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@Name", System.Data.SqlDbType.NVarChar) { Value = warehouse.Name });
            command.Parameters.Add(new SqlParameter("@Code", System.Data.SqlDbType.NVarChar) { Value = warehouse.Code });
            command.Parameters.Add(new SqlParameter("@Location", System.Data.SqlDbType.NVarChar) { Value = warehouse.Location });
            await connection.OpenAsync(ct);

            object result = await command.ExecuteScalarAsync(ct);
            if (result != null && result != DBNull.Value)
                newWarehouseId = Convert.ToInt32(result);

            return newWarehouseId;
        }
    }

    public async Task<bool> UpdateWarehouseAsync(int warehouseId, CreateWarehouseDto warehouse, CancellationToken ct)
    {
        string query = @"UPDATE Warehouses 
                        SET Name = @Name,
                            Code = @Code,
                            [Location] = @Location
                        WHERE WarehouseID = @WarehouseID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@WarehouseID", System.Data.SqlDbType.Int) { Value = warehouseId });
            command.Parameters.Add(new SqlParameter("@Name", System.Data.SqlDbType.NVarChar) { Value = warehouse.Name });
            command.Parameters.Add(new SqlParameter("@Code", System.Data.SqlDbType.NVarChar) { Value = warehouse.Code });
            command.Parameters.Add(new SqlParameter("@Location", System.Data.SqlDbType.NVarChar) { Value = warehouse.Location });

            await connection.OpenAsync(ct);

            int rowsAffected = await command.ExecuteNonQueryAsync(ct);

            return rowsAffected > 0;
        }
    }

    public async Task<bool> DeactivateWarehouseAsync(int warehouseId, CancellationToken ct)
    {
        string query = @"UPDATE Warehouses SET IsActive = 0 WHERE WarehouseID = @WarehouseID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@WarehouseId", System.Data.SqlDbType.Int) { Value = warehouseId });
            await connection.OpenAsync(ct);

            int rowsAffected = await command.ExecuteNonQueryAsync(ct);

            return rowsAffected > 0;
        }
    }

    private Warehouse MapReaderToWarehouse(SqlDataReader reader)
    {
        return new Warehouse()
        {
            WarehouseID = reader.GetInt32(reader.GetOrdinal("WarehouseID")),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            Code = reader.GetString(reader.GetOrdinal("Code")),
            Location = reader.GetString(reader.GetOrdinal("Location")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
        };
    }
}