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
        try
        {
            string query = @"SELECT UserID, Name, Username, Email, PasswordHash, IsActive, CreatedAt, RoleID FROM Users
                            WHERE UserID = @UserID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserID", userId);
                connection.Open();

                using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
                {
                    if (await reader.ReadAsync(ct))
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
                            RoleID = reader.GetInt32(reader.GetOrdinal("RoleID"))
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
            }

        }
        catch (Exception)
        {
            // TODO: Implement the logging functionality
            return null;
        }
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct)
    {
        try
        {
            string query = @"SELECT UserID, Name, Username, Email, PasswordHash, IsActive, CreatedAt, RoleID FROM Users
                            WHERE Username = @Username";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                connection.Open();

                using (SqlDataReader reader = await command.ExecuteReaderAsync(ct))
                {
                    if (await reader.ReadAsync(ct))
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
                            RoleID = reader.GetInt32(reader.GetOrdinal("RoleID"))
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
            }

        }
        catch (Exception)
        {
            // TODO: Implement the logging functionality
            return null;
        }
    }

    public async Task<int> AddNewUserAsync(CreateUserDto user, CancellationToken ct)
    {
        try
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

                object Result = await command.ExecuteScalarAsync(ct);

                if (Result != null && int.TryParse(Result.ToString(), out int InsertedID))
                    newUserId = InsertedID;

                return newUserId;
            }
        }
        catch (Exception)
        {
            // TODO: Implement the logging functionality
            return -1;
        }
    }

    public async Task<bool> UpdateUserAsync(int userId, CreateUserDto user, CancellationToken ct)
    {
        try
        {
            int rowsAffected = 0;
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

                rowsAffected = await command.ExecuteNonQueryAsync(ct);

                return rowsAffected > 0;
            }
        }
        catch (Exception)
        {
            // TODO: Implement the logging functionality
            return false;
        }
    }

    public async Task<bool> DeleteUserAsync(int userId, CancellationToken ct)
    {
        try
        {
            int rowsAffected = 0;
            string query = @"DELETE FROM Users WHERE UserID = @UserID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserID", userId);
                connection.Open();

                rowsAffected = await command.ExecuteNonQueryAsync(ct);

                return rowsAffected > 0;
            }
        }
        catch (Exception)
        {
            // TODO: Implement the logging functionality
            return false;
        }
    }

    public async Task<List<User>> GetAllUsersAsync(CancellationToken ct, int page = 1, int pageSize = 10)
    {
        try
        {
            List<User> users = new List<User>();
            string query = @"SELECT UserID, Name, Username, Email, PasswordHash, IsActive, CreatedAt, RoleID
                            FROM Users
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
                        User user = new User()
                        {
                            UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Username = reader.GetString(reader.GetOrdinal("Username")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                            RoleID = reader.GetInt32(reader.GetOrdinal("RoleID"))
                        };
                        
                        users.Add(user);
                    }
                    
                    return users;
                }
            }

        }
        catch (Exception)
        {
            // TODO: Implement the logging functionality
            return new List<User>();
        }
    }

    public async Task<int> GetUsersCountAsync(CancellationToken ct)
    {
        try
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
        catch (Exception)
        {
            // TODO: Implement the logging functionality
            return -1;
        }
    }

}
