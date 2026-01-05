namespace ElearningBackend.Models
{
    public class CourseModuleData
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Duration { get; set; }
        public List<LessonData> Lessons { get; set; } = new List<LessonData>();
    }

    public class LessonData
    {
        public string Name { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
    }
}
