namespace ElearningBackend.DTOs
{
    public class CartDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string? CourseThumbnail { get; set; }
        public string Instructor { get; set; } = string.Empty;
        public double? Duration { get; set; }
        public int TotalPrice { get; set; }
        public int TotalLessons { get; set; }
        public int TotalReviews { get; set; }
        public int? Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateCartDto
    {
        public long UserId { get; set; }
        public long CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string? CourseThumbnail { get; set; }
        public string Instructor { get; set; } = string.Empty;
        public double? Duration { get; set; }
        public int TotalPrice { get; set; }
        public int TotalLessons { get; set; }
        public int TotalReviews { get; set; }
        public int? Rating { get; set; }
    }

    public class UpdateCartDto
    {
        public string? CourseName { get; set; }
        public string? CourseThumbnail { get; set; }
        public string? Instructor { get; set; }
        public double? Duration { get; set; }
        public int? TotalPrice { get; set; }
        public int? TotalLessons { get; set; }
        public int? TotalReviews { get; set; }
        public int? Rating { get; set; }
    }

    public class FindCartDto
    {
        public long UserId { get; set; }
        public long CourseId { get; set; }
    }
}
