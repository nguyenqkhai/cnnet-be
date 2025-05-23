namespace be_net.Models.DTOs
{
    public class UserProfileDto
    {
        public long Id { get; set; }
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string? Avatar { get; set; }
        public string Role { get; set; } = null!;
    }

    public class UserProfileUpdateDto
    {
        public string? Username { get; set; }
        public string? Avatar { get; set; }
    }
}
