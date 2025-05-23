using be_net.Models;
using be_net.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace be_net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly CourseDBContext _context;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(CourseDBContext context, ILogger<ReviewController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviews()
        {
            try
            {
                var reviews = await _context.Reviews
                    .Where(r => !r.Destroy)
                    .Select(r => new ReviewDto
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
                    })
                    .ToListAsync();

                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByCourseId(long courseId)
        {
            try
            {
                var course = await _context.Courses.FindAsync(courseId);
                if (course == null || course.Destroy)
                    return NotFound("Course not found");

                var reviews = await _context.Reviews
                    .Where(r => r.CourseId == courseId && !r.Destroy)
                    .Select(r => new ReviewDto
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
                    })
                    .ToListAsync();

                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews by course id");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReviewDto>> CreateReview(ReviewCreateDto reviewCreateDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var user = await _context.Users.FindAsync(long.Parse(userId));
                if (user == null || user.Destroy)
                    return NotFound("User not found");

                var course = await _context.Courses.FindAsync(reviewCreateDto.CourseId);
                if (course == null || course.Destroy)
                    return NotFound("Course not found");

                // Check if user has already reviewed this course
                var existingReview = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.UserId == user.Id && r.CourseId == reviewCreateDto.CourseId && !r.Destroy);

                if (existingReview != null)
                    return BadRequest("You have already reviewed this course");

                // Check if rating is valid (1-5)
                if (reviewCreateDto.Rating < 1 || reviewCreateDto.Rating > 5)
                    return BadRequest("Rating must be between 1 and 5");

                var review = new Review
                {
                    UserId = user.Id,
                    UserAvatar = user.Avatar,
                    UserName = user.Username,
                    CourseId = reviewCreateDto.CourseId,
                    Content = reviewCreateDto.Content,
                    Rating = reviewCreateDto.Rating,
                    CreatedAt = DateTime.Now
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                var reviewDto = new ReviewDto
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

                return CreatedAtAction(nameof(GetReviewsByCourseId), new { courseId = review.CourseId }, reviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ReviewDto>> UpdateReview(long id, ReviewUpdateDto reviewUpdateDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var review = await _context.Reviews.FindAsync(id);

                if (review == null || review.Destroy)
                    return NotFound();

                // Check if the user is the owner of the review
                if (review.UserId != long.Parse(userId) && !User.IsInRole("admin"))
                    return Forbid();

                // Update only the properties that are not null
                if (reviewUpdateDto.Content != null)
                    review.Content = reviewUpdateDto.Content;
                if (reviewUpdateDto.Rating.HasValue)
                {
                    // Check if rating is valid (1-5)
                    if (reviewUpdateDto.Rating < 1 || reviewUpdateDto.Rating > 5)
                        return BadRequest("Rating must be between 1 and 5");

                    review.Rating = reviewUpdateDto.Rating.Value;
                }

                review.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                var reviewDto = new ReviewDto
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

                return Ok(reviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteReview(long id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var review = await _context.Reviews.FindAsync(id);

                if (review == null)
                    return NotFound();

                // Check if the user is the owner of the review or an admin
                if (review.UserId != long.Parse(userId) && !User.IsInRole("admin"))
                    return Forbid();

                // Soft delete
                review.Destroy = true;
                review.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Review deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }
    }
}
