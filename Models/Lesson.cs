using System;
using System.Collections.Generic;

namespace be_net.Models;

public partial class Lesson
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? VideoUrl { get; set; }

    public long CourseId { get; set; }

    public string? CoursePart { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool Destroy { get; set; }

    public virtual Course Course { get; set; } = null!;
}
