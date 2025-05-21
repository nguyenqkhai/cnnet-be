using System;
using System.Collections.Generic;

namespace be_net.Models;

public partial class Cart
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long CourseId { get; set; }

    public string CourseName { get; set; } = null!;

    public string? CourseThumbnail { get; set; }

    public string Instructor { get; set; } = null!;

    public double? Duration { get; set; }

    public int TotalPrice { get; set; }

    public int TotalLessons { get; set; }

    public int TotalReviews { get; set; }

    public int? Rating { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool Destroy { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
