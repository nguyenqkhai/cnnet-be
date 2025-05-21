using System;
using System.Collections.Generic;

namespace be_net.Models;

public partial class Voucher
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

    public bool Destroy { get; set; }
}
