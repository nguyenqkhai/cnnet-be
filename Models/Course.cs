using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ElearningBackend.Models
{
    [Table("Courses")]
    public class Course
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("name")]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("thumbnail")]
        [StringLength(255)]
        public string? Thumbnail { get; set; }

        [Required]
        [Column("instructor")]
        [StringLength(100)]
        public string Instructor { get; set; } = string.Empty;

        [Column("instructorRole")]
        [StringLength(100)]
        public string? InstructorRole { get; set; }

        [Column("instructorDescription")]
        public string? InstructorDescription { get; set; }

        [Column("duration")]
        public double? Duration { get; set; }

        [Column("price")]
        public int Price { get; set; } = 0;

        [Column("discount")]
        public int? Discount { get; set; } = 0;

        [Column("students")]
        public int Students { get; set; } = 0;

        [Column("courseModules")]
        public string? CourseModulesJson { get; set; }

        [NotMapped]
        public List<CourseModuleData> CourseModules
        {
            get
            {
                if (string.IsNullOrEmpty(CourseModulesJson))
                    return new List<CourseModuleData>();

                try
                {
                    return JsonConvert.DeserializeObject<List<CourseModuleData>>(CourseModulesJson) ?? new List<CourseModuleData>();
                }
                catch
                {
                    return new List<CourseModuleData>();
                }
            }
            set => CourseModulesJson = JsonConvert.SerializeObject(value);
        }

        [Column("category")]
        [StringLength(100)]
        public string? Category { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [Column("_destroy")]
        public bool Destroy { get; set; } = false;

        // Navigation properties
        public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
        public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
        public virtual ICollection<Module> Modules { get; set; } = new List<Module>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    }
}
