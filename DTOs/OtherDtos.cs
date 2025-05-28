namespace LmsBackend.DTOs
{
    // Contact DTOs
    public class ContactDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateContactDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    // Lesson DTOs
    public class LessonDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public long CourseId { get; set; }
        public string? CoursePart { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateLessonDto
    {
        public string Name { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public long CourseId { get; set; }
        public string? CoursePart { get; set; }
    }

    public class UpdateLessonDto
    {
        public string? Name { get; set; }
        public string? VideoUrl { get; set; }
        public string? CoursePart { get; set; }
    }

    // Module DTOs
    public class ModuleDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public double? Duration { get; set; }
        public List<long> Lessons { get; set; } = new List<long>();
        public long CourseId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateModuleDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public double? Duration { get; set; }
        public List<long> Lessons { get; set; } = new List<long>();
        public long CourseId { get; set; }
    }

    public class UpdateModuleDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public double? Duration { get; set; }
        public List<long>? Lessons { get; set; }
    }

    // Order DTOs
    public class OrderDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long CourseId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string? CourseThumbnail { get; set; }
        public string Instructor { get; set; } = string.Empty;
        public int TotalPrice { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateOrderDto
    {
        public long UserId { get; set; }
        public long CourseId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string? CourseThumbnail { get; set; }
        public string Instructor { get; set; } = string.Empty;
        public int TotalPrice { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }

    public class UpdateOrderDto
    {
        public string? Status { get; set; }
        public string? PaymentMethod { get; set; }
    }

    // Progress DTOs
    public class ProgressDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long CourseId { get; set; }
        public List<long> CompletedLessons { get; set; } = new List<long>();
        public int TotalLessons { get; set; }
        public int PercentComplete { get; set; }
        public DateTime? LastAccessedAt { get; set; }
    }

    public class InitProgressDto
    {
        public long UserId { get; set; }
        public long CourseId { get; set; }
        public int TotalLessons { get; set; }
    }

    public class UpdateProgressDto
    {
        public long LessonId { get; set; }
        public bool IsCompleted { get; set; }
    }

    // Review DTOs
    public class ReviewDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string? UserAvatar { get; set; }
        public string UserName { get; set; } = string.Empty;
        public long CourseId { get; set; }
        public string? Content { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateReviewDto
    {
        public long UserId { get; set; }
        public string? UserAvatar { get; set; }
        public string UserName { get; set; } = string.Empty;
        public long CourseId { get; set; }
        public string? Content { get; set; }
        public int Rating { get; set; }
    }

    public class UpdateReviewDto
    {
        public string? Content { get; set; }
        public int? Rating { get; set; }
    }

    // Voucher DTOs
    public class VoucherDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int Discount { get; set; }
        public List<long> CourseIds { get; set; } = new List<long>();
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public int? MinOrderValue { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateVoucherDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int Discount { get; set; }
        public List<long> CourseIds { get; set; } = new List<long>();
        public int? UsageLimit { get; set; }
        public int? MinOrderValue { get; set; }
        public DateTime? ExpiredAt { get; set; }
    }

    public class UpdateVoucherDto
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public int? Discount { get; set; }
        public List<long>? CourseIds { get; set; }
        public int? UsageLimit { get; set; }
        public int? MinOrderValue { get; set; }
        public DateTime? ExpiredAt { get; set; }
    }

    public class FindVoucherDto
    {
        public string Name { get; set; } = string.Empty;
    }

    // Wishlist DTOs
    public class WishlistDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string? CourseThumbnail { get; set; }
        public string Instructor { get; set; } = string.Empty;
        public double? Duration { get; set; }
        public int TotalPrice { get; set; }
        public int TotalLessons { get; set; }
        public int TotalReviews { get; set; }
        public int? Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateWishlistDto
    {
        public long UserId { get; set; }
        public long CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string? CourseThumbnail { get; set; }
        public string Instructor { get; set; } = string.Empty;
        public double? Duration { get; set; }
        public int TotalPrice { get; set; }
        public int TotalLessons { get; set; }
        public int TotalReviews { get; set; }
        public int? Rating { get; set; }
    }

    public class FindWishlistDto
    {
        public long UserId { get; set; }
        public long CourseId { get; set; }
    }

    // Media DTOs
    public class UploadResponseDto
    {
        public string Url { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long Size { get; set; }
    }

    // Payment DTOs
    public class PaymentRequestDto
    {
        public long OrderId { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string NotifyUrl { get; set; } = string.Empty;
    }

    public class PaymentResponseDto
    {
        public string PaymentUrl { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
