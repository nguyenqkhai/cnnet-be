using be_net.Models;
using be_net.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace be_net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleController : ControllerBase
    {
        private readonly CourseDBContext _context;
        private readonly ILogger<ModuleController> _logger;

        public ModuleController(CourseDBContext context, ILogger<ModuleController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModules()
        {
            try
            {
                var modules = await _context.Modules
                    .Where(m => !m.Destroy)
                    .Select(m => new ModuleDto
                    {
                        Id = m.Id,
                        Title = m.Title,
                        Description = m.Description,
                        Duration = m.Duration,
                        Lessons = m.Lessons,
                        CourseId = m.CourseId,
                        CreatedAt = m.CreatedAt,
                        UpdatedAt = m.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(modules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting modules");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModulesByCourseId(long courseId)
        {
            try
            {
                var course = await _context.Courses.FindAsync(courseId);
                if (course == null || course.Destroy)
                    return NotFound("Course not found");

                var modules = await _context.Modules
                    .Where(m => m.CourseId == courseId && !m.Destroy)
                    .Select(m => new ModuleDto
                    {
                        Id = m.Id,
                        Title = m.Title,
                        Description = m.Description,
                        Duration = m.Duration,
                        Lessons = m.Lessons,
                        CourseId = m.CourseId,
                        CreatedAt = m.CreatedAt,
                        UpdatedAt = m.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(modules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting modules by course id");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin,instructor")]
        public async Task<ActionResult<ModuleDto>> CreateModule(ModuleCreateDto moduleCreateDto)
        {
            try
            {
                var course = await _context.Courses.FindAsync(moduleCreateDto.CourseId);
                if (course == null || course.Destroy)
                    return NotFound("Course not found");

                var module = new Module
                {
                    Title = moduleCreateDto.Title,
                    Description = moduleCreateDto.Description,
                    Duration = moduleCreateDto.Duration,
                    Lessons = moduleCreateDto.Lessons,
                    CourseId = moduleCreateDto.CourseId,
                    CreatedAt = DateTime.Now
                };

                _context.Modules.Add(module);
                await _context.SaveChangesAsync();

                var moduleDto = new ModuleDto
                {
                    Id = module.Id,
                    Title = module.Title,
                    Description = module.Description,
                    Duration = module.Duration,
                    Lessons = module.Lessons,
                    CourseId = module.CourseId,
                    CreatedAt = module.CreatedAt,
                    UpdatedAt = module.UpdatedAt
                };

                return CreatedAtAction(nameof(GetModulesByCourseId), new { courseId = module.CourseId }, moduleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating module");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,instructor")]
        public async Task<ActionResult<ModuleDto>> UpdateModule(long id, ModuleUpdateDto moduleUpdateDto)
        {
            try
            {
                var module = await _context.Modules.FindAsync(id);

                if (module == null || module.Destroy)
                    return NotFound();

                // Update only the properties that are not null
                if (moduleUpdateDto.Title != null)
                    module.Title = moduleUpdateDto.Title;
                if (moduleUpdateDto.Description != null)
                    module.Description = moduleUpdateDto.Description;
                if (moduleUpdateDto.Duration.HasValue)
                    module.Duration = moduleUpdateDto.Duration;
                if (moduleUpdateDto.Lessons != null)
                    module.Lessons = moduleUpdateDto.Lessons;

                module.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                var moduleDto = new ModuleDto
                {
                    Id = module.Id,
                    Title = module.Title,
                    Description = module.Description,
                    Duration = module.Duration,
                    Lessons = module.Lessons,
                    CourseId = module.CourseId,
                    CreatedAt = module.CreatedAt,
                    UpdatedAt = module.UpdatedAt
                };

                return Ok(moduleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating module");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,instructor")]
        public async Task<ActionResult> DeleteModule(long id)
        {
            try
            {
                var module = await _context.Modules.FindAsync(id);

                if (module == null)
                    return NotFound();

                // Soft delete
                module.Destroy = true;
                module.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Module deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting module");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }
    }
}
