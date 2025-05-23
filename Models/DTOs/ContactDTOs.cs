using System;

namespace be_net.Models.DTOs
{
    public class ContactDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string Subject { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool Destroy { get; set; }
    }

    public class ContactCreateDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string Subject { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
