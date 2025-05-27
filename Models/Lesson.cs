using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LmsBackend.Models
{
    [Table("Lessons")]
    public class Lesson
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("name")]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [Column("video_url")]
        [StringLength(255)]
        public string? VideoUrl { get; set; }

        [Column("courseId")]
        public long CourseId { get; set; }

        [Column("coursePart")]
        [StringLength(100)]
        public string? CoursePart { get; set; }

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
