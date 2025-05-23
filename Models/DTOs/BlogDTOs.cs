using System;

namespace be_net.Models.DTOs
{
    public class BlogDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Summary { get; set; }
        public string Content { get; set; } = null!;
        public string? Tags { get; set; }
        public string? CoverImage { get; set; }
        public string Author { get; set; } = null!;
        public long AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class BlogCreateDto
    {
        public string Title { get; set; } = null!;
        public string? Summary { get; set; }
        public string Content { get; set; } = null!;
        public string? Tags { get; set; }
        public string? CoverImage { get; set; }
    }

    public class BlogUpdateDto
    {
        public string? Title { get; set; }
        public string? Summary { get; set; }
        public string? Content { get; set; }
        public string? Tags { get; set; }
        public string? CoverImage { get; set; }
    }
}
