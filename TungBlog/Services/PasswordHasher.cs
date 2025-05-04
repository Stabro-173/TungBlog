using System.Security.Cryptography;
using System.Text;

namespace TungBlog.Services
{
    public class PasswordHasher
    {
        public static string HashPassword(string password) // Hàm này dùng để mã hóa mật khẩu thành một chuỗi không thể đảo ngược
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hashedPassword;
        }
    }
} 