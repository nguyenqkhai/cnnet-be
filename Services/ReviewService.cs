using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ElearningBackend.Data;
using ElearningBackend.DTOs;
using ElearningBackend.Models;

namespace ElearningBackend.Services
{
    public class ReviewService : IReviewService
    {
        private readonly LmsDbContext _context;
        private readonly IMapper _mapper;

        public ReviewService(LmsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ReviewDto>> GetAllReviewsAsync()
        {
            var reviews = await _context.Reviews
                .Where(r => !r.Destroy)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                UserId = r.UserId,
                UserAvatar = r.UserAvatar,
                UserName = r.UserName,
                CourseId = r.CourseId,
                Content = r.Content,
                Rating = r.Rating,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            }).ToList();
        }

        public async Task<ReviewDto> CreateReviewAsync(CreateReviewDto createReviewDto)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == createReviewDto.UserId && !u.Destroy);
            var courseExists = await _context.Courses.AnyAsync(c => c.Id == createReviewDto.CourseId && !c.Destroy);
            if (!userExists)
                throw new NotFoundException("User not found");
            if (!courseExists)
                throw new NotFoundException("Course not found");
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == createReviewDto.UserId &&
                                         r.CourseId == createReviewDto.CourseId &&
                                         !r.Destroy);

            if (existingReview != null)
            {
                throw new InvalidOperationException("User has already reviewed this course");
            }
            var hasPurchased = await _context.Orders
                .AnyAsync(o => o.UserId == createReviewDto.UserId &&
                              o.CourseId == createReviewDto.CourseId &&
                              o.Status == "COMPLETED" &&
                              !o.Destroy);
            if (!hasPurchased)
            {
                // Check if user has any order for this course (even PENDING) - temporary fix
                var hasAnyOrder = await _context.Orders
                    .AnyAsync(o => o.UserId == createReviewDto.UserId &&
                                  o.CourseId == createReviewDto.CourseId &&
                                  !o.Destroy);

                if (!hasAnyOrder)
                {
                    throw new InvalidOperationException("User must purchase the course before reviewing");
                }

                // If user has PENDING order, allow review (temporary fix for payment flow)
                Console.WriteLine($"🔍 User has PENDING order, allowing review for testing");
            }

            var review = new Review
            {
                UserId = createReviewDto.UserId,
                UserAvatar = createReviewDto.UserAvatar,
                UserName = createReviewDto.UserName,
                CourseId = createReviewDto.CourseId,
                Content = createReviewDto.Content,
                Rating = createReviewDto.Rating,
                CreatedAt = DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return new ReviewDto
            {
                Id = review.Id,
                UserId = review.UserId,
                UserAvatar = review.UserAvatar,
                UserName = review.UserName,
                CourseId = review.CourseId,
                Content = review.Content,
                Rating = review.Rating,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt
            };
        }

        public async Task<List<ReviewDto>> GetReviewsByCourseIdAsync(long courseId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.CourseId == courseId && !r.Destroy)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                UserId = r.UserId,
                UserAvatar = r.UserAvatar,
                UserName = r.UserName,
                CourseId = r.CourseId,
                Content = r.Content,
                Rating = r.Rating,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            }).ToList();
        }

        public async Task<ReviewDto> UpdateReviewAsync(long id, UpdateReviewDto updateReviewDto)
        {
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == id && !r.Destroy);

            if (review == null)
            {
                throw new NotFoundException("Review not found");
            }

            if (!string.IsNullOrEmpty(updateReviewDto.Content))
                review.Content = updateReviewDto.Content;
            if (updateReviewDto.Rating.HasValue)
                review.Rating = updateReviewDto.Rating.Value;

            review.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new ReviewDto
            {
                Id = review.Id,
                UserId = review.UserId,
                UserAvatar = review.UserAvatar,
                UserName = review.UserName,
                CourseId = review.CourseId,
                Content = review.Content,
                Rating = review.Rating,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt
            };
        }

        public async Task<bool> DeleteReviewAsync(long id)
        {
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == id && !r.Destroy);

            if (review == null)
            {
                return false;
            }

            review.Destroy = true;
            review.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
