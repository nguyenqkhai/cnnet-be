using ElearningBackend.Models;

namespace ElearningBackend.DTOs
{
    public class CourseDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Thumbnail { get; set; }
        public string Instructor { get; set; } = string.Empty;
        public string? InstructorRole { get; set; }
        public string? InstructorDescription { get; set; }
        public double? Duration { get; set; }
        public int Price { get; set; }
        public int? Discount { get; set; }
        public int Students { get; set; }
        public List<CourseModuleData> CourseModules { get; set; } = new List<CourseModuleData>();
        public string? Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateCourseDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Thumbnail { get; set; }
        public string Instructor { get; set; } = string.Empty;
        public string? InstructorRole { get; set; }
        public string? InstructorDescription { get; set; }
        public double? Duration { get; set; }
        public int Price { get; set; }
        public int? Discount { get; set; }
        public List<CourseModuleData> CourseModules { get; set; } = new List<CourseModuleData>();
        public string? Category { get; set; }
    }

    public class UpdateCourseDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Thumbnail { get; set; }
        public string? Instructor { get; set; }
        public string? InstructorRole { get; set; }
        public string? InstructorDescription { get; set; }
        public double? Duration { get; set; }
        public int? Price { get; set; }
        public int? Discount { get; set; }
        public List<CourseModuleData>? CourseModules { get; set; }
        public string? Category { get; set; }
    }
}
