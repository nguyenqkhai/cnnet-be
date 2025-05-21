using System;
using System.Collections.Generic;

namespace be_net.Models;

public partial class Module
{
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public double? Duration { get; set; }

    public string? Lessons { get; set; }

    public long CourseId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool Destroy { get; set; }

    public virtual Course Course { get; set; } = null!;
}
