using System;

namespace be_net.Models.DTOs
{
    public class CartDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long CourseId { get; set; }
        public string CourseName { get; set; } = null!;
        public string? CourseThumbnail { get; set; }
        public string Instructor { get; set; } = null!;
        public double? Duration { get; set; }
        public int TotalPrice { get; set; }
        public int TotalLessons { get; set; }
        public int TotalReviews { get; set; }
        public int? Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CartCreateDto
    {
        public long CourseId { get; set; }
    }

    public class CartUpdateDto
    {
        public int? TotalPrice { get; set; }
    }

    public class CartFindDto
    {
        public long UserId { get; set; }
        public long CourseId { get; set; }
    }
}
