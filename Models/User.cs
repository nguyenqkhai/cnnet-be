using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace be_net.Models;

public partial class User
{
    public long Id { get; set; }
    public string Email { get; set; } = null!;
    [Column(TypeName = "nvarchar(500)")]
    public string Password { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string? Avatar { get; set; }
    public string Role { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool Destroy { get; set; }
    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}