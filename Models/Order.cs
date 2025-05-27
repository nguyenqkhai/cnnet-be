using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LmsBackend.Models
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("userId")]
        public long UserId { get; set; }

        [Column("courseId")]
        public long CourseId { get; set; }

        [Required]
        [Column("userEmail")]
        [StringLength(255)]
        public string UserEmail { get; set; } = string.Empty;

        [Required]
        [Column("userName")]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

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

        [Column("totalPrice")]
        public int TotalPrice { get; set; }

        [Required]
        [Column("paymentMethod")]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required]
        [Column("status")]
        [StringLength(50)]
        public string Status { get; set; } = "pending";

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
