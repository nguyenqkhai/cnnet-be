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
    public class CourseController : ControllerBase
    {
        private readonly CourseDBContext _context;
        private readonly ILogger<CourseController> _logger;

        public CourseController(CourseDBContext context, ILogger<CourseController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
        {
            try
            {
                var courses = await _context.Courses
                    .Where(c => !c.Destroy)
                    .Select(c => new CourseDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        Thumbnail = c.Thumbnail,
                        Instructor = c.Instructor,
                        InstructorRole = c.InstructorRole,
                        InstructorDescription = c.InstructorDescription,
                        Duration = c.Duration,
                        Price = c.Price,
                        Discount = c.Discount,
                        Students = c.Students,
                        CourseModules = c.CourseModules,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting courses");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCourseById(long id)
        {
            try
            {
                var course = await _context.Courses
                    .Where(c => c.Id == id && !c.Destroy)
                    .Select(c => new CourseDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        Thumbnail = c.Thumbnail,
                        Instructor = c.Instructor,
                        InstructorRole = c.InstructorRole,
                        InstructorDescription = c.InstructorDescription,
                        Duration = c.Duration,
                        Price = c.Price,
                        Discount = c.Discount,
                        Students = c.Students,
                        CourseModules = c.CourseModules,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (course == null)
                    return NotFound();

                return Ok(course);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting course by id");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin,instructor")]
        public async Task<ActionResult<CourseDto>> CreateCourse(CourseCreateDto courseCreateDto)
        {
            try
            {
                var course = new Course
                {
                    Name = courseCreateDto.Name,
                    Description = courseCreateDto.Description,
                    Thumbnail = courseCreateDto.Thumbnail,
                    Instructor = courseCreateDto.Instructor,
                    InstructorRole = courseCreateDto.InstructorRole,
                    InstructorDescription = courseCreateDto.InstructorDescription,
                    Duration = courseCreateDto.Duration,
                    Price = courseCreateDto.Price,
                    Discount = courseCreateDto.Discount,
                    Students = 0,
                    CourseModules = courseCreateDto.CourseModules,
                    CreatedAt = DateTime.Now
                };

                _context.Courses.Add(course);
                await _context.SaveChangesAsync();

                var courseDto = new CourseDto
                {
                    Id = course.Id,
                    Name = course.Name,
                    Description = course.Description,
                    Thumbnail = course.Thumbnail,
                    Instructor = course.Instructor,
                    InstructorRole = course.InstructorRole,
                    InstructorDescription = course.InstructorDescription,
                    Duration = course.Duration,
                    Price = course.Price,
                    Discount = course.Discount,
                    Students = course.Students,
                    CourseModules = course.CourseModules,
                    CreatedAt = course.CreatedAt,
                    UpdatedAt = course.UpdatedAt
                };

                return CreatedAtAction(nameof(GetCourseById), new { id = course.Id }, courseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,instructor")]
        public async Task<ActionResult<CourseDto>> UpdateCourse(long id, CourseUpdateDto courseUpdateDto)
        {
            try
            {
                var course = await _context.Courses.FindAsync(id);

                if (course == null || course.Destroy)
                    return NotFound();

                // Update only the properties that are not null
                if (courseUpdateDto.Name != null)
                    course.Name = courseUpdateDto.Name;
                if (courseUpdateDto.Description != null)
                    course.Description = courseUpdateDto.Description;
                if (courseUpdateDto.Thumbnail != null)
                    course.Thumbnail = courseUpdateDto.Thumbnail;
                if (courseUpdateDto.Instructor != null)
                    course.Instructor = courseUpdateDto.Instructor;
                if (courseUpdateDto.InstructorRole != null)
                    course.InstructorRole = courseUpdateDto.InstructorRole;
                if (courseUpdateDto.InstructorDescription != null)
                    course.InstructorDescription = courseUpdateDto.InstructorDescription;
                if (courseUpdateDto.Duration.HasValue)
                    course.Duration = courseUpdateDto.Duration;
                if (courseUpdateDto.Price.HasValue)
                    course.Price = courseUpdateDto.Price.Value;
                if (courseUpdateDto.Discount.HasValue)
                    course.Discount = courseUpdateDto.Discount;
                if (courseUpdateDto.CourseModules != null)
                    course.CourseModules = courseUpdateDto.CourseModules;

                course.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                var courseDto = new CourseDto
                {
                    Id = course.Id,
                    Name = course.Name,
                    Description = course.Description,
                    Thumbnail = course.Thumbnail,
                    Instructor = course.Instructor,
                    InstructorRole = course.InstructorRole,
                    InstructorDescription = course.InstructorDescription,
                    Duration = course.Duration,
                    Price = course.Price,
                    Discount = course.Discount,
                    Students = course.Students,
                    CourseModules = course.CourseModules,
                    CreatedAt = course.CreatedAt,
                    UpdatedAt = course.UpdatedAt
                };

                return Ok(courseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating course");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,instructor")]
        public async Task<ActionResult> DeleteCourse(long id)
        {
            try
            {
                var course = await _context.Courses.FindAsync(id);

                if (course == null)
                    return NotFound();

                // Soft delete
                course.Destroy = true;
                course.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Course deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }
    }
}
