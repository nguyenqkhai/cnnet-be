namespace ElearningBackend.DTOs
{
    public class BlogDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string Content { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();
        public string? CoverImage { get; set; }
        public string Author { get; set; } = string.Empty;
        public long AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateBlogDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string Content { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();
        public string? CoverImage { get; set; }
        public string Author { get; set; } = string.Empty;
        public long AuthorId { get; set; }
    }

    public class UpdateBlogDto
    {
        public string? Title { get; set; }
        public string? Summary { get; set; }
        public string? Content { get; set; }
        public List<string>? Tags { get; set; }
        public string? CoverImage { get; set; }
        public string? Author { get; set; }
    }
}
