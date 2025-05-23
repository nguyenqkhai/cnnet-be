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
    [Authorize]
    public class ProgressController : ControllerBase
    {
        private readonly CourseDBContext _context;
        private readonly ILogger<ProgressController> _logger;

        public ProgressController(CourseDBContext context, ILogger<ProgressController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProgressDto>>> GetAllProgress([FromQuery] long? userId = null)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                    return Unauthorized();

                // If userId is provided, ensure the user is only accessing their own progress or is an admin
                if (userId.HasValue && userId != long.Parse(currentUserId) && !User.IsInRole("admin"))
                    return Forbid();

                // If userId is not provided, use the current user's ID
                var userIdToUse = userId ?? long.Parse(currentUserId);

                var progresses = await _context.Progresses
                    .Where(p => p.UserId == userIdToUse)
                    .Select(p => new ProgressDto
                    {
                        Id = p.Id,
                        UserId = p.UserId,
                        CourseId = p.CourseId,
                        CompletedLessons = p.CompletedLessons,
                        TotalLessons = p.TotalLessons,
                        PercentComplete = p.PercentComplete,
                        LastAccessedAt = p.LastAccessedAt
                    })
                    .ToListAsync();

                return Ok(progresses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all progress");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpGet("{courseId}")]
        public async Task<ActionResult<ProgressDto>> GetCourseProgress(long courseId, [FromQuery] long? userId = null)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                    return Unauthorized();

                // If userId is provided, ensure the user is only accessing their own progress or is an admin
                if (userId.HasValue && userId != long.Parse(currentUserId) && !User.IsInRole("admin"))
                    return Forbid();

                // If userId is not provided, use the current user's ID
                var userIdToUse = userId ?? long.Parse(currentUserId);

                var course = await _context.Courses.FindAsync(courseId);
                if (course == null || course.Destroy)
                    return NotFound("Course not found");

                var progress = await _context.Progresses
                    .Where(p => p.UserId == userIdToUse && p.CourseId == courseId)
                    .Select(p => new ProgressDto
                    {
                        Id = p.Id,
                        UserId = p.UserId,
                        CourseId = p.CourseId,
                        CompletedLessons = p.CompletedLessons,
                        TotalLessons = p.TotalLessons,
                        PercentComplete = p.PercentComplete,
                        LastAccessedAt = p.LastAccessedAt
                    })
                    .FirstOrDefaultAsync();

                if (progress == null)
                    return NotFound("Progress not found");

                return Ok(progress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting course progress");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpPost("init")]
        public async Task<ActionResult<ProgressDto>> InitCourseProgress(ProgressInitDto progressInitDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var user = await _context.Users.FindAsync(long.Parse(userId));
                if (user == null || user.Destroy)
                    return NotFound("User not found");

                var course = await _context.Courses.FindAsync(progressInitDto.CourseId);
                if (course == null || course.Destroy)
                    return NotFound("Course not found");

                // Check if progress already exists
                var existingProgress = await _context.Progresses
                    .FirstOrDefaultAsync(p => p.UserId == user.Id && p.CourseId == progressInitDto.CourseId);

                if (existingProgress != null)
                    return BadRequest("Progress already initialized for this course");

                // Check if user has purchased the course
                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.UserId == user.Id && o.CourseId == progressInitDto.CourseId && 
                                         o.Status == "completed" && !o.Destroy);

                if (order == null)
                    return BadRequest("You need to purchase this course first");

                // Count total lessons
                var totalLessons = await _context.Lessons
                    .CountAsync(l => l.CourseId == course.Id && !l.Destroy);

                var progress = new Progress
                {
                    UserId = user.Id,
                    CourseId = course.Id,
                    CompletedLessons = "",
                    TotalLessons = totalLessons,
                    PercentComplete = 0,
                    LastAccessedAt = DateTime.Now
                };

                _context.Progresses.Add(progress);
                await _context.SaveChangesAsync();

                var progressDto = new ProgressDto
                {
                    Id = progress.Id,
                    UserId = progress.UserId,
                    CourseId = progress.CourseId,
                    CompletedLessons = progress.CompletedLessons,
                    TotalLessons = progress.TotalLessons,
                    PercentComplete = progress.PercentComplete,
                    LastAccessedAt = progress.LastAccessedAt
                };

                return CreatedAtAction(nameof(GetCourseProgress), new { courseId = progress.CourseId }, progressDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing course progress");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpPost("update-lesson")]
        public async Task<ActionResult<ProgressDto>> UpdateLessonProgress(LessonProgressUpdateDto lessonProgressUpdateDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var user = await _context.Users.FindAsync(long.Parse(userId));
                if (user == null || user.Destroy)
                    return NotFound("User not found");

                var course = await _context.Courses.FindAsync(lessonProgressUpdateDto.CourseId);
                if (course == null || course.Destroy)
                    return NotFound("Course not found");

                var lesson = await _context.Lessons.FindAsync(lessonProgressUpdateDto.LessonId);
                if (lesson == null || lesson.Destroy)
                    return NotFound("Lesson not found");

                // Ensure the lesson belongs to the course
                if (lesson.CourseId != course.Id)
                    return BadRequest("Lesson does not belong to the specified course");

                var progress = await _context.Progresses
                    .FirstOrDefaultAsync(p => p.UserId == user.Id && p.CourseId == lessonProgressUpdateDto.CourseId);

                if (progress == null)
                    return NotFound("Progress not found. Initialize progress first.");

                // Update completed lessons
                var completedLessons = string.IsNullOrEmpty(progress.CompletedLessons) 
                    ? new List<long>() 
                    : progress.CompletedLessons.Split(',').Select(long.Parse).ToList();

                if (lessonProgressUpdateDto.Completed)
                {
                    if (!completedLessons.Contains(lessonProgressUpdateDto.LessonId))
                    {
                        completedLessons.Add(lessonProgressUpdateDto.LessonId);
                    }
                }
                else
                {
                    completedLessons.Remove(lessonProgressUpdateDto.LessonId);
                }

                progress.CompletedLessons = string.Join(",", completedLessons);
                progress.PercentComplete = (int)Math.Round((double)completedLessons.Count / progress.TotalLessons * 100);
                progress.LastAccessedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                var progressDto = new ProgressDto
                {
                    Id = progress.Id,
                    UserId = progress.UserId,
                    CourseId = progress.CourseId,
                    CompletedLessons = progress.CompletedLessons,
                    TotalLessons = progress.TotalLessons,
                    PercentComplete = progress.PercentComplete,
                    LastAccessedAt = progress.LastAccessedAt
                };

                return Ok(progressDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lesson progress");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }
    }
}
