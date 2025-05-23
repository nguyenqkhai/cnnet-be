using System;

namespace be_net.Models.DTOs
{
    public class VoucherDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public int Discount { get; set; }
        public string? CourseIds { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public int? MinOrderValue { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class VoucherCreateDto
    {
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public int Discount { get; set; }
        public string? CourseIds { get; set; }
        public int? UsageLimit { get; set; }
        public int? MinOrderValue { get; set; }
        public DateTime? ExpiredAt { get; set; }
    }

    public class VoucherUpdateDto
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public int? Discount { get; set; }
        public string? CourseIds { get; set; }
        public int? UsageLimit { get; set; }
        public int? MinOrderValue { get; set; }
        public DateTime? ExpiredAt { get; set; }
    }

    public class VoucherFindDto
    {
        public string Code { get; set; } = null!;
        public long CourseId { get; set; }
    }
}
