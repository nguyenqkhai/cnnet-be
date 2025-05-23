using System;

namespace be_net.Models.DTOs
{
    public class LessonDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? VideoUrl { get; set; }
        public long CourseId { get; set; }
        public string? CoursePart { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class LessonCreateDto
    {
        public string Name { get; set; } = null!;
        public string? VideoUrl { get; set; }
        public long CourseId { get; set; }
        public string? CoursePart { get; set; }
    }

    public class LessonUpdateDto
    {
        public string? Name { get; set; }
        public string? VideoUrl { get; set; }
        public string? CoursePart { get; set; }
    }
}
