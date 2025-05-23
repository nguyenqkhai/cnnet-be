using System;

namespace be_net.Models.DTOs
{
    public class ProgressDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long CourseId { get; set; }
        public string? CompletedLessons { get; set; }
        public int TotalLessons { get; set; }
        public int PercentComplete { get; set; }
        public DateTime? LastAccessedAt { get; set; }
    }

    public class ProgressInitDto
    {
        public long CourseId { get; set; }
    }

    public class LessonProgressUpdateDto
    {
        public long CourseId { get; set; }
        public long LessonId { get; set; }
        public bool Completed { get; set; }
    }
}
