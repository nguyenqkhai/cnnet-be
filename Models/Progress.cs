using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace LmsBackend.Models
{
    [Table("Progress")]
    public class Progress
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("userId")]
        public long UserId { get; set; }

        [Column("courseId")]
        public long CourseId { get; set; }

        [Column("completedLessons")]
        public string? CompletedLessonsJson { get; set; }

        [NotMapped]
        public List<long> CompletedLessons
        {
            get => string.IsNullOrEmpty(CompletedLessonsJson) ? new List<long>() : JsonConvert.DeserializeObject<List<long>>(CompletedLessonsJson) ?? new List<long>();
            set => CompletedLessonsJson = JsonConvert.SerializeObject(value);
        }

        [Column("totalLessons")]
        public int TotalLessons { get; set; } = 0;

        [Column("percentComplete")]
        public int PercentComplete { get; set; } = 0;

        [Column("lastAccessedAt")]
        public DateTime? LastAccessedAt { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; } = null!;
    }
}
