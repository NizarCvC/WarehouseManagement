using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;
using WarehouseDataAccess.Interfaces;

namespace WarehouseDataAccess.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly string _connectionString;

    public CustomerRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<Customer?> GetCustomerByIdAsync(int customerId, CancellationToken ct)
    {
        string query = @"SELECT c.CustomerID, c.Name, c.Phone, c.Email, c.Address, c.IsActive, c.CreatedAt 
                        FROM Customers c 
                        WHERE c.CustomerID = @CustomerID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.Int) { Value = customerId });
            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                if (await reader.ReadAsync(ct))
                    return MapReaderToCustomer(reader);
                else
                    return null;
            }
        }
    }

    public async Task<Customer?> GetCustomerByEmailAsync(string email, CancellationToken ct)
    {
        string query = @"SELECT c.CustomerID, c.Name, c.Phone, c.Email, c.Address, c.IsActive, c.CreatedAt 
                        FROM Customers c 
                        WHERE c.Email = @Email";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar) { Value = email });

            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                if (await reader.ReadAsync(ct))
                    return MapReaderToCustomer(reader);
                else
                    return null;
            }
        }
    }

    public async Task<Customer?> GetCustomerByPhoneAsync(string phone, CancellationToken ct)
    {
        string query = @"SELECT c.CustomerID, c.Name, c.Phone, c.Email, c.Address, c.IsActive, c.CreatedAt 
                        FROM Customers c 
                        WHERE c.Phone = @Phone";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@Phone", SqlDbType.VarChar) { Value = phone });

            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                if (await reader.ReadAsync(ct))
                    return MapReaderToCustomer(reader);
                else
                    return null;
            }
        }
    }

    public async Task<List<Customer>> GetAllCustomersAsync(CancellationToken ct, int page = 1, int pageSize = 10)
    {
        List<Customer> customers = new List<Customer>();
        string query = @"SELECT c.CustomerID, c.Name, c.Phone, c.Email, c.Address, c.IsActive, c.CreatedAt 
                        FROM Customers c
                        WHERE c.IsActive = 1
                        ORDER BY c.CustomerID
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
                    Customer customer = MapReaderToCustomer(reader);
                    customers.Add(customer);
                }

                return customers;
            }
        }
    }

    public async Task<int> GetCustomersCountAsync(CancellationToken ct)
    {
        int countNumber = 0;
        string query = @"SELECT COUNT(*) FROM Customers";

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

    public async Task<int> AddNewCustomerAsync(CreateCustomerDto customer, CancellationToken ct)
    {
        int newCustomerId = -1;
        string query = @"INSERT INTO Customers (Name, Phone, Email, Address)
                        VALUES (@Name, @Phone, @Email, @Address)
                        SELECT SCOPE_IDENTITY()";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar) { Value = customer.Name });
            command.Parameters.Add(new SqlParameter("@Phone", SqlDbType.VarChar) { Value = customer.Phone });
            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar) { Value = customer.Email });
            command.Parameters.Add(new SqlParameter("@Address", SqlDbType.NVarChar) { Value = customer.Address });

            await connection.OpenAsync(ct);

            object result = await command.ExecuteScalarAsync(ct);
            if (result != null && result != DBNull.Value)
                newCustomerId = Convert.ToInt32(result);

            return newCustomerId;
        }
    }

    public async Task<bool> UpdateCustomerAsync(int customerId, CreateCustomerDto customer, CancellationToken ct)
    {
        string query = @"UPDATE Customers 
                        SET Name = @Name,
                            Phone = @Phone,
                            Email = @Email,
                            Address = @Address
                        WHERE CustomerID = @CustomerID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.Int) { Value = customerId });
            command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar) { Value = customer.Name });
            command.Parameters.Add(new SqlParameter("@Phone", SqlDbType.VarChar) { Value = customer.Phone });
            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar) { Value = customer.Email });
            command.Parameters.Add(new SqlParameter("@Address", SqlDbType.NVarChar) { Value = customer.Address });

            await connection.OpenAsync(ct);

            int rowsAffected = await command.ExecuteNonQueryAsync(ct);

            return rowsAffected > 0;
        }
    }

    public async Task<bool> DeactivateCustomerAsync(int customerId, CancellationToken ct)
    {
        string query = @"UPDATE Customers SET IsActive = 0 WHERE CustomerID = @CustomerID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.Int) { Value = customerId });
            await connection.OpenAsync(ct);

            int rowsAffected = await command.ExecuteNonQueryAsync(ct);

            return rowsAffected > 0;
        }
    }

    private Customer MapReaderToCustomer(SqlDataReader reader)
    {
        return new Customer()
        {
            CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            Phone = reader.GetString(reader.GetOrdinal("Phone")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            Address = reader.GetString(reader.GetOrdinal("Address")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
        };
    }

    public async Task<bool> IsCustomerExistsByIdAsync(int customerId, CancellationToken ct)
    {
        string query = @"SELECT 1 FROM Customers WHERE CustomerID = @CustomerID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.Int) { Value = customerId });
            await connection.OpenAsync(ct);

            object? result = await command.ExecuteScalarAsync(ct);
            return result != null;
        }
    }

    public async Task<bool> IsCustomerExistsByEmailAsync(string email, CancellationToken ct)
    {
        string query = @"SELECT 1 FROM Customers WHERE Email = @Email";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar) { Value = email });
            await connection.OpenAsync(ct);

            object? result = await command.ExecuteScalarAsync(ct);
            return result != null;
        }
    }   

    public async Task<bool> IsCustomerExistsByPhoneAsync(string phone, CancellationToken ct)
    {
        string query = @"SELECT 1 FROM Customers WHERE Phone = @Phone";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@Phone", SqlDbType.VarChar) { Value = phone });
            await connection.OpenAsync(ct);

            object? result = await command.ExecuteScalarAsync(ct);
            return result != null;
        }
    }
}