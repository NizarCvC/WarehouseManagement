using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.Entities;
using WarehouseDataAccess.Interfaces;
namespace WarehouseDataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }

    public async Task<User?> GetUserByIdAsync(int userId, CancellationToken ct)
    {
        string query = @"SELECT u.UserID, u.Name, u.Username, u.Email, u.PasswordHash, u.IsActive, u.CreatedAt, u.RoleID, r.Name AS RoleName FROM Users u
                            INNER JOIN Roles r ON u.RoleID = r.RoleID
                            WHERE UserID = @UserID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@UserID", userId);
            connection.Open();

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
        string query = @"SELECT u.UserID, u.Name, u.Username, u.Email, u.PasswordHash, u.IsActive, u.CreatedAt, u.RoleID, r.Name AS RoleName FROM Users u
                            INNER JOIN Roles r ON u.RoleID = r.RoleID
                            WHERE Username = @Username";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@Username", username);
            connection.Open();

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
        string query = @"SELECT u.UserID, u.Name, u.Username, u.Email, u.PasswordHash,
                            u.IsActive, u.CreatedAt, u.RoleID, r.Name AS RoleName
                            FROM Users u
                            INNER JOIN Roles r ON u.RoleID = r.RoleID
                            ORDER BY UserID
                            OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
                            FETCH NEXT @RowsPerPage ROWS ONLY;";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@PageNumber", page);
            command.Parameters.AddWithValue("@RowsPerPage", pageSize);
            connection.Open();

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
            connection.Open();

            object Result = await command.ExecuteScalarAsync(ct);

            if (Result != null && int.TryParse(Result.ToString(), out int countNum))
                countNumber = countNum;

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
            command.Parameters.AddWithValue("@Name", user.Name);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", user.Password);
            command.Parameters.AddWithValue("@RoleID", user.RoleID);
            connection.Open();

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
            command.Parameters.AddWithValue("@UserID", userId);
            command.Parameters.AddWithValue("@Name", user.Name);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", user.Password);
            command.Parameters.AddWithValue("@RoleID", user.RoleID);
            connection.Open();

            int rowsAffected = await command.ExecuteNonQueryAsync(ct);

            return rowsAffected > 0;
        }
    }

    public async Task<bool> DeleteUserAsync(int userId, CancellationToken ct)
    {
        string query = @"UPDATE Users SET IsActive = 0 WHERE UserID = @UserID";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@UserID", userId);
            connection.Open();

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
            command.Parameters.AddWithValue("@UserID", userId);
            connection.Open();

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
            command.Parameters.AddWithValue("@Username", username);
            connection.Open();

            object? result = await command.ExecuteScalarAsync(ct);
            return result != null;
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
            RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
            Role = new Role()
            {
                RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                Name = reader.GetString(reader.GetOrdinal("RoleName"))
            }
        };
    }
}
