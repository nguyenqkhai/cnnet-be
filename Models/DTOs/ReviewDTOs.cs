using System;

namespace be_net.Models.DTOs
{
    public class ReviewDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string? UserAvatar { get; set; }
        public string UserName { get; set; } = null!;
        public long CourseId { get; set; }
        public string? Content { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ReviewCreateDto
    {
        public long CourseId { get; set; }
        public string? Content { get; set; }
        public int Rating { get; set; }
    }

    public class ReviewUpdateDto
    {
        public string? Content { get; set; }
        public int? Rating { get; set; }
    }
}
