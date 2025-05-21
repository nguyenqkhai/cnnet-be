using System;
using System.Collections.Generic;

namespace be_net.Models;

public partial class Progress
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long CourseId { get; set; }

    public string? CompletedLessons { get; set; }

    public int TotalLessons { get; set; }

    public int PercentComplete { get; set; }

    public DateTime? LastAccessedAt { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
