using System;
using System.Collections.Generic;

namespace be_net.Models;

public partial class Blog
{
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Summary { get; set; }

    public string Content { get; set; } = null!;

    public string? Tags { get; set; }

    public string? CoverImage { get; set; }

    public string Author { get; set; } = null!;

    public long AuthorId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool Destroy { get; set; }

    public virtual User AuthorNavigation { get; set; } = null!;
}
