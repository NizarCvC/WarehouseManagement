using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;
using WarehouseCore.enums;
using WarehouseDataAccess.Interfaces;
namespace WarehouseDataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<User?> GetUserByIdAsync(int userId, CancellationToken ct)
    {
        string query = @"SELECT u.UserID, u.Name, u.Username, u.Email, u.PasswordHash, u.IsActive, u.CreatedAt,
                        u.RoleID, u.RefreshToken, u.RefreshTokenExpiryTime
                        FROM Users u WHERE UserID = @UserID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int) { Value = userId });
            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                if (await reader.ReadAsync(ct))
                    return MapReaderToUser(reader);
                else
                    return null;
            }
        }
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct)
    {
        string query = @"SELECT u.UserID, u.Name, u.Username, u.Email, u.PasswordHash, u.IsActive, u.CreatedAt,
                        u.RoleID, u.RefreshToken, u.RefreshTokenExpiryTime
                        FROM Users u WHERE Username = @Username";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar) { Value = username });
            await connection.OpenAsync(ct);

            using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
            {
                if (await reader.ReadAsync(ct))
                    return MapReaderToUser(reader);
                else
                    return null;
            }
        }
    }

    public async Task<List<User>> GetAllUsersAsync(CancellationToken ct, int page = 1, int pageSize = 10)
    {
        List<User> users = new List<User>();
        string query = @"SELECT u.UserID, u.Name, u.Username, u.Email, u.PasswordHash, u.IsActive, u.CreatedAt, 
                            u.RoleID, u.RefreshToken, u.RefreshTokenExpiryTime
                            FROM Users u
                            WHERE u.IsActive = 1
                            ORDER BY UserID
                            OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
                            FETCH NEXT @RowsPerPage ROWS ONLY;";

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
                    User user = MapReaderToUser(reader);
                    users.Add(user);
                }

                return users;
            }
        }
    }

    public async Task<int> GetUsersCountAsync(CancellationToken ct)
    {
        int countNumber = 0;
        string query = @"SELECT COUNT(*) FROM Users";

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

    public async Task<int> AddNewUserAsync(CreateUserDto user, CancellationToken ct)
    {
        int newUserId = -1;
        string query = @"INSERT INTO Users (Name, Username, Email, PasswordHash, RoleID)
                        VALUES (@Name, @Username, @Email, @PasswordHash, @RoleID)
                        SELECT SCOPE_IDENTITY()";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar) { Value = user.Name });
            command.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar) { Value = user.Username });
            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar) { Value = user.Email });
            command.Parameters.Add(new SqlParameter("@PasswordHash", SqlDbType.NVarChar) { Value = user.Password });
            command.Parameters.Add(new SqlParameter("@RoleID", SqlDbType.Int) { Value = user.RoleID });
            await connection.OpenAsync(ct);

            object result = await command.ExecuteScalarAsync(ct);
            if (result != null && result != DBNull.Value)
                newUserId = Convert.ToInt32(result);

            return newUserId;
        }
    }

    public async Task<bool> UpdateUserAsync(int userId, CreateUserDto user, CancellationToken ct)
    {
        string query = @"UPDATE Users 
                            SET Name = @Name,
                                Username = @Username,
                                Email = @Email,
                                PasswordHash = @PasswordHash,
                                RoleID = @RoleID
                            WHERE UserID = @UserID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int) { Value = userId });
            command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar) { Value = user.Name });
            command.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar) { Value = user.Username });
            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar) { Value = user.Email });
            command.Parameters.Add(new SqlParameter("@PasswordHash", SqlDbType.NVarChar) { Value = user.Password });
            command.Parameters.Add(new SqlParameter("@RoleID", SqlDbType.Int) { Value = user.RoleID });
            await connection.OpenAsync(ct);

            int rowsAffected = await command.ExecuteNonQueryAsync(ct);

            return rowsAffected > 0;
        }
    }

    public async Task<bool> DeactivateUserAsync(int userId, CancellationToken ct)
    {
        string query = @"UPDATE Users SET IsActive = 0 WHERE UserID = @UserID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int) { Value = userId });
            await connection.OpenAsync(ct);

            int rowsAffected = await command.ExecuteNonQueryAsync(ct);

            return rowsAffected > 0;
        }
    }

    public async Task<bool> IsUserIdExists(int userId, CancellationToken ct)
    {
        string query = @"SELECT 1 FROM Users WHERE UserID = @UserID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int) { Value = userId });
            await connection.OpenAsync(ct);

            object? result = await command.ExecuteScalarAsync(ct);
            return result != null;
        }
    }

    public async Task<bool> IsUsernameExists(string username, CancellationToken ct)
    {
        string query = @"SELECT 1 FROM Users WHERE Username = @Username";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar) { Value = username });
            await connection.OpenAsync(ct);

            object? result = await command.ExecuteScalarAsync(ct);
            return result != null;
        }
    }

    public async Task<bool> UpdateRefreshTokenAsync(int userId, string? refreshToken, DateTime? expiryTime, CancellationToken ct)
    {
        string query = @"UPDATE Users 
                        SET RefreshToken = @RefreshToken, 
                        RefreshTokenExpiryTime = @ExpiryTime 
                        WHERE UserID = @UserID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int) { Value = userId });
            command.Parameters.Add(new SqlParameter("@RefreshToken", SqlDbType.NVarChar) { Value = (object?)refreshToken ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@ExpiryTime", SqlDbType.DateTime) { Value = (object?)expiryTime ?? DBNull.Value });

            await connection.OpenAsync(ct);

            int rowsAffected = await command.ExecuteNonQueryAsync(ct);
            return rowsAffected > 0;
        }
    }

    private User MapReaderToUser(SqlDataReader reader)
    {
        return new User()
        {
            UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            Username = reader.GetString(reader.GetOrdinal("Username")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            Role = (enRole)reader.GetInt32(reader.GetOrdinal("RoleID")),
            RefreshToken = reader.IsDBNull(reader.GetOrdinal("RefreshToken"))
                ? null
                : reader.GetString(reader.GetOrdinal("RefreshToken")),
            RefreshTokenExpiryTime = reader.IsDBNull(reader.GetOrdinal("RefreshTokenExpiryTime"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("RefreshTokenExpiryTime"))
        };
    }

}