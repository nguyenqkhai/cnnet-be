using System;
using System.Collections.Generic;

namespace be_net.Models.DTOs
{
    public class CourseDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Thumbnail { get; set; }
        public string Instructor { get; set; } = null!;
        public string? InstructorRole { get; set; }
        public string? InstructorDescription { get; set; }
        public double? Duration { get; set; }
        public int Price { get; set; }
        public int? Discount { get; set; }
        public int Students { get; set; }
        public string? CourseModules { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CourseCreateDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Thumbnail { get; set; }
        public string Instructor { get; set; } = null!;
        public string? InstructorRole { get; set; }
        public string? InstructorDescription { get; set; }
        public double? Duration { get; set; }
        public int Price { get; set; }
        public int? Discount { get; set; }
        public string? CourseModules { get; set; }
    }

    public class CourseUpdateDto
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
        public string? CourseModules { get; set; }
    }
}
