using be_net.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace be_net.Services
{
    public interface IAuthService
    {
        string GenerateJwtToken(User user);
        string HashPassword(string password);
        bool VerifyPassword(string hashedPassword, string providedPassword);
    }

    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("JwtSettings:Secret").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string HashPassword(string password)
        {
            // Sử dụng HMACSHA512 để hash mật khẩu
            using (var hmac = new HMACSHA512())
            {
                var salt = hmac.Key; // HMACSHA512 có salt 64 bytes
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Kết hợp salt và hash
                var hashBytes = new byte[salt.Length + hash.Length];
                Array.Copy(salt, 0, hashBytes, 0, salt.Length);
                Array.Copy(hash, 0, hashBytes, salt.Length, hash.Length);

                return Convert.ToBase64String(hashBytes);
            }
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            try
            {
                // Giải mã Base64 thành mảng byte
                var hashBytes = Convert.FromBase64String(hashedPassword);
                
                // HMACSHA512 có salt 64 bytes
                var salt = new byte[64];
                
                // Đảm bảo hash có đủ độ dài
                if (hashBytes.Length < salt.Length)
                    return false;

                // Trích xuất salt từ hash
                Array.Copy(hashBytes, 0, salt, 0, salt.Length);
                
                // Sử dụng HMACSHA512 với salt đã trích xuất để tạo hash mới
                using (var hmac = new HMACSHA512(salt))
                {
                    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(providedPassword));
                    
                    if (hashBytes.Length < salt.Length + computedHash.Length)
                        return false;
                    
                    for (int i = 0; i < computedHash.Length; i++)
                    {
                        if (computedHash[i] != hashBytes[salt.Length + i])
                            return false;
                    }
                }
                
                // Nếu tất cả các byte đều khớp, mật khẩu là đúng
                return true;
            }
            catch (Exception)
            {
                // Nếu có bất kỳ lỗi nào, trả về false
                return false;
            }
        }
    }
}