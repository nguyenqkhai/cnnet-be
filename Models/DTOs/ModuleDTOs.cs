using System;

namespace be_net.Models.DTOs
{
    public class ModuleDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public double? Duration { get; set; }
        public string? Lessons { get; set; }
        public long CourseId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ModuleCreateDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public double? Duration { get; set; }
        public string? Lessons { get; set; }
        public long CourseId { get; set; }
    }

    public class ModuleUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public double? Duration { get; set; }
        public string? Lessons { get; set; }
    }
}
