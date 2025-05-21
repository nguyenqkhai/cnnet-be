using System;
using System.Collections.Generic;

namespace be_net.Models;

public partial class Course
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Thumbnail { get; set; }

    public string Instructor { get; set; } = null!;

    public string? InstructorRole { get; set; }

    public string? InstructorDescription { get; set; }

    public double? Duration { get; set; }

    public int Price { get; set; }

    public int? Discount { get; set; }

    public int Students { get; set; }

    public string? CourseModules { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool Destroy { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public virtual ICollection<Module> Modules { get; set; } = new List<Module>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
