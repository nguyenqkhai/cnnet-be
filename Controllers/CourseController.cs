using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ElearningBackend.DTOs;
using ElearningBackend.Services;

namespace ElearningBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CourseDto>>> GetAllCourses()
        {
            try
            {
                var courses = await _courseService.GetAllCoursesAsync();
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách khóa học", details = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin,instructor")]
        public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CreateCourseDto createCourseDto)
        {
            try
            {
                // Debug: Log dữ liệu nhận được
                Console.WriteLine($"Received CourseModules count: {createCourseDto.CourseModules?.Count}");
                if (createCourseDto.CourseModules != null)
                {
                    foreach (var module in createCourseDto.CourseModules)
                    {
                        Console.WriteLine($"Module: {module.Title}, Lessons count: {module.Lessons?.Count}");
                        if (module.Lessons != null)
                        {
                            foreach (var lesson in module.Lessons)
                            {
                                Console.WriteLine($"  Lesson: {lesson.Name}, VideoUrl: {lesson.VideoUrl}");
                            }
                        }
                    }
                }

                var course = await _courseService.CreateCourseAsync(createCourseDto);
                return Ok(course);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo khóa học", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCourseById(long id)
        {
            try
            {
                var course = await _courseService.GetCourseByIdAsync(id);
                return Ok(course);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy khóa học", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,instructor")]
        public async Task<ActionResult<CourseDto>> UpdateCourse(long id, [FromBody] UpdateCourseDto updateCourseDto)
        {
            try
            {
                var course = await _courseService.UpdateCourseAsync(id, updateCourseDto);
                return Ok(course);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật khóa học", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteCourse(long id)
        {
            try
            {
                var result = await _courseService.DeleteCourseAsync(id);
                if (result)
                {
                    return Ok(new { message = "Đã xóa khóa học thành công" });
                }
                return NotFound(new { message = "Không tìm thấy khóa học" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa khóa học", details = ex.Message });
            }
        }
    }
}
