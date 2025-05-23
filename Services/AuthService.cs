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

            var secretKey = _configuration.GetSection("JwtSettings:Secret").Value ??
                throw new InvalidOperationException("JWT Secret key is not configured");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

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
            byte[] salt = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using (var hmac = new HMACSHA256(salt))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                byte[] hashBytes = new byte[salt.Length + hash.Length];
                Array.Copy(salt, 0, hashBytes, 0, salt.Length);
                Array.Copy(hash, 0, hashBytes, salt.Length, hash.Length);

                return Convert.ToBase64String(hashBytes);
            }
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            try
            {
                byte[] hashBytes = Convert.FromBase64String(hashedPassword);
                if (hashBytes.Length < 64)
                {
                    Console.WriteLine("Invalid hash length");
                    return false;
                }

                byte[] salt = new byte[32];
                Array.Copy(hashBytes, 0, salt, 0, 32);
                Console.WriteLine($"Salt length: {salt.Length}");

                using (var hmac = new HMACSHA256(salt))
                {
                    byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(providedPassword));
                    Console.WriteLine($"Computed hash length: {computedHash.Length}");

                    for (int i = 0; i < computedHash.Length; i++)
                    {
                        if (computedHash[i] != hashBytes[32 + i])
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}