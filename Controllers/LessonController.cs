using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LmsBackend.DTOs;
using LmsBackend.Services;

namespace LmsBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;

        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        [HttpGet]
        [Authorize(Roles = "admin,instructor")]
        public async Task<ActionResult<List<LessonDto>>> GetAllLessons()
        {
            try
            {
                var lessons = await _lessonService.GetAllLessonsAsync();
                return Ok(lessons);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách bài học", details = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin,instructor")]
        public async Task<ActionResult<LessonDto>> CreateLesson([FromBody] CreateLessonDto createLessonDto)
        {
            try
            {
                var lesson = await _lessonService.CreateLessonAsync(createLessonDto);
                return Ok(lesson);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo bài học", details = ex.Message });
            }
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<List<LessonDto>>> GetLessonsByCourseId(long courseId)
        {
            try
            {
                var lessons = await _lessonService.GetLessonsByCourseIdAsync(courseId);
                return Ok(lessons);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách bài học theo khóa học", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,instructor")]
        public async Task<ActionResult<LessonDto>> UpdateLesson(long id, [FromBody] UpdateLessonDto updateLessonDto)
        {
            try
            {
                var lesson = await _lessonService.UpdateLessonAsync(id, updateLessonDto);
                return Ok(lesson);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật bài học", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteLesson(long id)
        {
            try
            {
                var result = await _lessonService.DeleteLessonAsync(id);
                if (result)
                {
                    return Ok(new { message = "Đã xóa bài học thành công" });
                }
                return NotFound(new { message = "Không tìm thấy bài học" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa bài học", details = ex.Message });
            }
        }
    }
}
