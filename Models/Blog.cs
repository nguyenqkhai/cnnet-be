using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ElearningBackend.Models
{
    [Table("Blogs")]
    public class Blog
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("title")]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [Column("summary")]
        [StringLength(500)]
        public string? Summary { get; set; }

        [Required]
        [Column("content")]
        public string Content { get; set; } = string.Empty;

        [Column("tags")]
        public string? TagsJson { get; set; }

        [NotMapped]
        public List<string> Tags
        {
            get => string.IsNullOrEmpty(TagsJson) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(TagsJson) ?? new List<string>();
            set => TagsJson = JsonConvert.SerializeObject(value);
        }

        [Column("coverImage")]
        [StringLength(255)]
        public string? CoverImage { get; set; }

        [Required]
        [Column("author")]
        [StringLength(100)]
        public string Author { get; set; } = string.Empty;

        [Column("authorId")]
        public long AuthorId { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [Column("_destroy")]
        public bool Destroy { get; set; } = false;

        // Navigation properties
        [ForeignKey("AuthorId")]
        public virtual User AuthorUser { get; set; } = null!;
    }
}
