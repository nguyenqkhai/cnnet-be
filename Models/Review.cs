using System;
using System.Collections.Generic;

namespace be_net.Models;

public partial class Review
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string? UserAvatar { get; set; }

    public string UserName { get; set; } = null!;

    public long CourseId { get; set; }

    public string? Content { get; set; }

    public int Rating { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool Destroy { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
