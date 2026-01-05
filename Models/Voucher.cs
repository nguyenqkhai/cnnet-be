using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ElearningBackend.Models
{
    [Table("Vouchers")]
    public class Voucher
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("name")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("code")]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Column("discount")]
        public int Discount { get; set; }

        [Column("courseIds")]
        public string? CourseIdsJson { get; set; }

        [NotMapped]
        public List<long> CourseIds
        {
            get
            {
                if (string.IsNullOrEmpty(CourseIdsJson))
                    return new List<long>();

                try
                {
                    return JsonConvert.DeserializeObject<List<long>>(CourseIdsJson) ?? new List<long>();
                }
                catch
                {
                    try
                    {
                        var stringIds = JsonConvert.DeserializeObject<List<string>>(CourseIdsJson);
                        if (stringIds != null)
                        {
                            var longIds = new List<long>();
                            foreach (var stringId in stringIds)
                            {
                                if (long.TryParse(stringId, out long longId))
                                {
                                    longIds.Add(longId);
                                }
                            }
                            return longIds;
                        }
                    }
                    catch
                    {
                    }
                    return new List<long>();
                }
            }
            set => CourseIdsJson = JsonConvert.SerializeObject(value);
        }

        [Column("usageLimit")]
        public int? UsageLimit { get; set; }

        [Column("usedCount")]
        public int UsedCount { get; set; } = 0;

        [Column("minOrderValue")]
        public int? MinOrderValue { get; set; }

        [Column("expiredAt")]
        public DateTime? ExpiredAt { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [Column("_destroy")]
        public bool Destroy { get; set; } = false;
    }
}
