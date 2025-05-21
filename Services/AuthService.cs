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
            // Sử dụng HMACSHA256 thay vì HMACSHA512
            using (var hmac = new HMACSHA256())
            {
                var salt = hmac.Key; // HMACSHA256 có salt 32 bytes
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
                var hashBytes = Convert.FromBase64String(hashedPassword);
                
                // Kiểm tra độ dài của chuỗi hash để xác định thuật toán
                if (hashBytes.Length >= 128) // Có thể là HMACSHA512 (64 bytes salt + 64 bytes hash)
                {
                    return VerifyWithHMACSHA512(hashBytes, providedPassword);
                }
                else // Có thể là HMACSHA256 (32 bytes salt + 32 bytes hash)
                {
                    return VerifyWithHMACSHA256(hashBytes, providedPassword);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool VerifyWithHMACSHA256(byte[] hashBytes, string providedPassword)
        {
            try
            {
                // HMACSHA256 có salt 32 bytes
                var salt = new byte[32];
                if (hashBytes.Length < salt.Length)
                    return false;
                    
                Array.Copy(hashBytes, 0, salt, 0, salt.Length);
                
                using (var hmac = new HMACSHA256(salt))
                {
                    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(providedPassword));
                    
                    if (hashBytes.Length < salt.Length + computedHash.Length)
                        return false;
                        
                    for (int i = 0; i < computedHash.Length; i++)
                    {
                        if (computedHash[i] != hashBytes[salt.Length + i])
                            return false;
                    }
                    
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private bool VerifyWithHMACSHA512(byte[] hashBytes, string providedPassword)
        {
            try
            {
                // HMACSHA512 có salt 64 bytes
                var salt = new byte[64];
                if (hashBytes.Length < salt.Length)
                    return false;
                    
                Array.Copy(hashBytes, 0, salt, 0, salt.Length);
                
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
                    
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}