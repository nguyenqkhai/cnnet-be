namespace be_net.Models.DTOs
{
    public class RegisterDto
    {
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Avatar { get; set; }
    }

    public class LoginDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class UserDto
    {
        public long Id { get; set; }
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string? Avatar { get; set; }
        public string Role { get; set; } = null!;
        public string Token { get; set; } = null!;
    }

    public class ResetPasswordTempDto
    {
        public string Email { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}