using System.Security.Cryptography;
using System.Text;

namespace Registro.Classes
{
    /// <summary>
    /// A helper class for hashing passwords using the SHA-256 cryptographic algorithm.
    /// </summary>
    internal class PasswordHelper
    {
        /// <summary>
        /// Hashes the given password using the SHA-256 algorithm and returns the hashed value as a hexadecimal string.
        /// </summary>
        /// <param name="password">The password to be hashed.</param>
        /// <returns>A hexadecimal string representation of the hashed password.</returns>
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
