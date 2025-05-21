using System;
using System.Collections.Generic;

namespace be_net.Models;

public partial class Order
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long CourseId { get; set; }

    public string UserEmail { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string CourseName { get; set; } = null!;

    public string? CourseThumbnail { get; set; }

    public string Instructor { get; set; } = null!;

    public int TotalPrice { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool Destroy { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
