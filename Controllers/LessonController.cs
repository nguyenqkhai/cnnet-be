using be_net.Models;
using be_net.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace be_net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly CourseDBContext _context;
        private readonly ILogger<LessonController> _logger;

        public LessonController(CourseDBContext context, ILogger<LessonController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LessonDto>>> GetLessons()
        {
            try
            {
                var lessons = await _context.Lessons
                    .Where(l => !l.Destroy)
                    .Select(l => new LessonDto
                    {
                        Id = l.Id,
                        Name = l.Name,
                        VideoUrl = l.VideoUrl,
                        CourseId = l.CourseId,
                        CoursePart = l.CoursePart,
                        CreatedAt = l.CreatedAt,
                        UpdatedAt = l.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(lessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lessons");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<LessonDto>>> GetLessonsByCourseId(long courseId)
        {
            try
            {
                var course = await _context.Courses.FindAsync(courseId);
                if (course == null || course.Destroy)
                    return NotFound("Course not found");

                var lessons = await _context.Lessons
                    .Where(l => l.CourseId == courseId && !l.Destroy)
                    .Select(l => new LessonDto
                    {
                        Id = l.Id,
                        Name = l.Name,
                        VideoUrl = l.VideoUrl,
                        CourseId = l.CourseId,
                        CoursePart = l.CoursePart,
                        CreatedAt = l.CreatedAt,
                        UpdatedAt = l.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(lessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lessons by course id");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin,instructor")]
        public async Task<ActionResult<LessonDto>> CreateLesson(LessonCreateDto lessonCreateDto)
        {
            try
            {
                var course = await _context.Courses.FindAsync(lessonCreateDto.CourseId);
                if (course == null || course.Destroy)
                    return NotFound("Course not found");

                var lesson = new Lesson
                {
                    Name = lessonCreateDto.Name,
                    VideoUrl = lessonCreateDto.VideoUrl,
                    CourseId = lessonCreateDto.CourseId,
                    CoursePart = lessonCreateDto.CoursePart,
                    CreatedAt = DateTime.Now
                };

                _context.Lessons.Add(lesson);
                await _context.SaveChangesAsync();

                var lessonDto = new LessonDto
                {
                    Id = lesson.Id,
                    Name = lesson.Name,
                    VideoUrl = lesson.VideoUrl,
                    CourseId = lesson.CourseId,
                    CoursePart = lesson.CoursePart,
                    CreatedAt = lesson.CreatedAt,
                    UpdatedAt = lesson.UpdatedAt
                };

                return CreatedAtAction(nameof(GetLessonsByCourseId), new { courseId = lesson.CourseId }, lessonDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lesson");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,instructor")]
        public async Task<ActionResult<LessonDto>> UpdateLesson(long id, LessonUpdateDto lessonUpdateDto)
        {
            try
            {
                var lesson = await _context.Lessons.FindAsync(id);

                if (lesson == null || lesson.Destroy)
                    return NotFound();

                // Update only the properties that are not null
                if (lessonUpdateDto.Name != null)
                    lesson.Name = lessonUpdateDto.Name;
                if (lessonUpdateDto.VideoUrl != null)
                    lesson.VideoUrl = lessonUpdateDto.VideoUrl;
                if (lessonUpdateDto.CoursePart != null)
                    lesson.CoursePart = lessonUpdateDto.CoursePart;

                lesson.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                var lessonDto = new LessonDto
                {
                    Id = lesson.Id,
                    Name = lesson.Name,
                    VideoUrl = lesson.VideoUrl,
                    CourseId = lesson.CourseId,
                    CoursePart = lesson.CoursePart,
                    CreatedAt = lesson.CreatedAt,
                    UpdatedAt = lesson.UpdatedAt
                };

                return Ok(lessonDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lesson");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,instructor")]
        public async Task<ActionResult> DeleteLesson(long id)
        {
            try
            {
                var lesson = await _context.Lessons.FindAsync(id);

                if (lesson == null)
                    return NotFound();

                // Soft delete
                lesson.Destroy = true;
                lesson.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Lesson deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting lesson");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }
    }
}
