using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace be_net.Models;

public partial class CourseDBContext : DbContext
{
    public CourseDBContext()
    {
    }

    public CourseDBContext(DbContextOptions<CourseDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Progress> Progresses { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Blogs__3213E83F47DB6F82");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Author)
                .HasMaxLength(100)
                .HasColumnName("author");
            entity.Property(e => e.AuthorId).HasColumnName("authorId");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CoverImage)
                .HasMaxLength(255)
                .HasColumnName("coverImage");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Destroy).HasColumnName("_destroy");
            entity.Property(e => e.Summary)
                .HasMaxLength(500)
                .HasColumnName("summary");
            entity.Property(e => e.Tags).HasColumnName("tags");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.AuthorNavigation).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Blogs_User");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Carts__3213E83F1A53FE3A");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CourseId).HasColumnName("courseId");
            entity.Property(e => e.CourseName)
                .HasMaxLength(255)
                .HasColumnName("courseName");
            entity.Property(e => e.CourseThumbnail)
                .HasMaxLength(255)
                .HasColumnName("courseThumbnail");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Destroy).HasColumnName("_destroy");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.Instructor)
                .HasMaxLength(100)
                .HasColumnName("instructor");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.TotalLessons).HasColumnName("totalLessons");
            entity.Property(e => e.TotalPrice).HasColumnName("totalPrice");
            entity.Property(e => e.TotalReviews).HasColumnName("totalReviews");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Course).WithMany(p => p.Carts)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Carts_Courses");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Carts_User");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Contacts__3213E83FFB27E44D");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Destroy).HasColumnName("_destroy");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Courses__3213E83FA2BA6E4D");

            entity.HasIndex(e => e.Name, "IX_Course_Name");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CourseModules).HasColumnName("courseModules");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Destroy).HasColumnName("_destroy");
            entity.Property(e => e.Discount)
                .HasDefaultValue(0)
                .HasColumnName("discount");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.Instructor)
                .HasMaxLength(100)
                .HasColumnName("instructor");
            entity.Property(e => e.InstructorDescription).HasColumnName("instructorDescription");
            entity.Property(e => e.InstructorRole)
                .HasMaxLength(100)
                .HasColumnName("instructorRole");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Students).HasColumnName("students");
            entity.Property(e => e.Thumbnail)
                .HasMaxLength(255)
                .HasColumnName("thumbnail");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Lessons__3213E83F3069372B");

            entity.HasIndex(e => e.CourseId, "IX_Lessons_Course");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CourseId).HasColumnName("courseId");
            entity.Property(e => e.CoursePart)
                .HasMaxLength(100)
                .HasColumnName("coursePart");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Destroy).HasColumnName("_destroy");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
            entity.Property(e => e.VideoUrl)
                .HasMaxLength(255)
                .HasColumnName("video_url");

            entity.HasOne(d => d.Course).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lessons_Courses");
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Modules__3213E83F103A4830");

            entity.HasIndex(e => e.CourseId, "IX_Modules_Course");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CourseId).HasColumnName("courseId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Destroy).HasColumnName("_destroy");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.Lessons).HasColumnName("lessons");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Course).WithMany(p => p.Modules)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Modules_Courses");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Orders__3213E83FCB2047AA");

            entity.HasIndex(e => new { e.UserId, e.CourseId }, "IX_Orders_UserCourse");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CourseId).HasColumnName("courseId");
            entity.Property(e => e.CourseName)
                .HasMaxLength(255)
                .HasColumnName("courseName");
            entity.Property(e => e.CourseThumbnail)
                .HasMaxLength(255)
                .HasColumnName("courseThumbnail");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Destroy).HasColumnName("_destroy");
            entity.Property(e => e.Instructor)
                .HasMaxLength(100)
                .HasColumnName("instructor");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasColumnName("paymentMethod");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("pending")
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice).HasColumnName("totalPrice");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(255)
                .HasColumnName("userEmail");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .HasColumnName("userName");

            entity.HasOne(d => d.Course).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Courses");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_User");
        });

        modelBuilder.Entity<Progress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Progress__3213E83FC7CA75F9");

            entity.ToTable("Progress");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompletedLessons).HasColumnName("completedLessons");
            entity.Property(e => e.CourseId).HasColumnName("courseId");
            entity.Property(e => e.LastAccessedAt)
                .HasColumnType("datetime")
                .HasColumnName("lastAccessedAt");
            entity.Property(e => e.PercentComplete).HasColumnName("percentComplete");
            entity.Property(e => e.TotalLessons).HasColumnName("totalLessons");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Course).WithMany(p => p.Progresses)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Progress_Course");

            entity.HasOne(d => d.User).WithMany(p => p.Progresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Progress_User");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reviews__3213E83F12AC6C80");

            entity.HasIndex(e => e.CourseId, "IX_Reviews_Course");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CourseId).HasColumnName("courseId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Destroy).HasColumnName("_destroy");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
            entity.Property(e => e.UserAvatar)
                .HasMaxLength(255)
                .HasColumnName("userAvatar");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .HasColumnName("userName");

            entity.HasOne(d => d.Course).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reviews_Courses");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reviews_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3213E83FB51CFA82");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "IX_User_Email");

            entity.HasIndex(e => e.Username, "IX_User_Username");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .HasColumnName("avatar");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Destroy).HasColumnName("_destroy");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(8000)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasDefaultValue("student")
                .HasColumnName("role");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Vouchers__3213E83F681B94E8");

            entity.HasIndex(e => e.Code, "UQ__Vouchers__357D4CF97C488B16").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.CourseIds).HasColumnName("courseIds");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Destroy).HasColumnName("_destroy");
            entity.Property(e => e.Discount).HasColumnName("discount");
            entity.Property(e => e.ExpiredAt)
                .HasColumnType("datetime")
                .HasColumnName("expiredAt");
            entity.Property(e => e.MinOrderValue).HasColumnName("minOrderValue");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
            entity.Property(e => e.UsageLimit).HasColumnName("usageLimit");
            entity.Property(e => e.UsedCount).HasColumnName("usedCount");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
