using System.Security.Cryptography;
using System.Text;

namespace WarehouseServices.Security;

public static class Hashing
{
    public static string HashPasswordOnly(string password)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(password);

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(inputBytes);
            return Convert.ToHexString(hashBytes);
        }
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        string hashOfInput = HashPasswordOnly(password);
        return string.Equals(hashOfInput, storedHash, StringComparison.OrdinalIgnoreCase);
    }
}
