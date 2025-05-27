using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LmsBackend.Models
{
    [Table("Wishlists")]
    public class Wishlist
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("userId")]
        public long UserId { get; set; }

        [Column("courseId")]
        public long CourseId { get; set; }

        [Required]
        [Column("courseName")]
        [StringLength(255)]
        public string CourseName { get; set; } = string.Empty;

        [Column("courseThumbnail")]
        [StringLength(255)]
        public string? CourseThumbnail { get; set; }

        [Required]
        [Column("instructor")]
        [StringLength(100)]
        public string Instructor { get; set; } = string.Empty;

        [Column("duration")]
        public double? Duration { get; set; }

        [Column("totalPrice")]
        public int TotalPrice { get; set; }

        [Column("totalLessons")]
        public int TotalLessons { get; set; } = 0;

        [Column("totalReviews")]
        public int TotalReviews { get; set; } = 0;

        [Column("rating")]
        public int? Rating { get; set; }

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
