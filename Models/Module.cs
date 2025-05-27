using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace LmsBackend.Models
{
    [Table("Modules")]
    public class Module
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("title")]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("duration")]
        public double? Duration { get; set; }

        [Column("lessons")]
        public string? LessonsJson { get; set; }

        [NotMapped]
        public List<long> Lessons
        {
            get => string.IsNullOrEmpty(LessonsJson) ? new List<long>() : JsonConvert.DeserializeObject<List<long>>(LessonsJson) ?? new List<long>();
            set => LessonsJson = JsonConvert.SerializeObject(value);
        }

        [Column("courseId")]
        public long CourseId { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [Column("_destroy")]
        public bool Destroy { get; set; } = false;

        // Navigation properties
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; } = null!;
    }
}
