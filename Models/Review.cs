using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LmsBackend.Models
{
    [Table("Reviews")]
    public class Review
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("userId")]
        public long UserId { get; set; }

        [Column("userAvatar")]
        [StringLength(255)]
        public string? UserAvatar { get; set; }

        [Required]
        [Column("userName")]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Column("courseId")]
        public long CourseId { get; set; }

        [Column("content")]
        public string? Content { get; set; }

        [Column("rating")]
        public int Rating { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [Column("_destroy")]
        public bool Destroy { get; set; } = false;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; } = null!;
    }
}
