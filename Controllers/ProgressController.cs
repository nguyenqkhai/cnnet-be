using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ElearningBackend.DTOs;
using ElearningBackend.Services;

namespace ElearningBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProgressController : ControllerBase
    {
        private readonly IProgressService _progressService;

        public ProgressController(IProgressService progressService)
        {
            _progressService = progressService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProgressDto>>> GetUserProgress()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                var progresses = await _progressService.GetProgressByUserIdAsync(userId);
                return Ok(progresses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách tiến độ", details = ex.Message });
            }
        }

        [HttpGet("{courseId}")]
        public async Task<ActionResult<ProgressDto>> GetProgressByCourseId(long courseId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                var progress = await _progressService.GetProgressByCourseIdAsync(userId, courseId);
                if (progress == null)
                {
                    return NotFound(new { message = "Không tìm thấy tiến độ cho khóa học này" });
                }

                return Ok(progress);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy tiến độ của khóa học", details = ex.Message });
            }
        }

        [HttpPost("init")]
        public async Task<ActionResult<ProgressDto>> InitializeProgress([FromBody] InitProgressDto initProgressDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                initProgressDto.UserId = userId; 

                var progress = await _progressService.InitializeProgressAsync(initProgressDto);
                return Ok(progress);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi khởi tạo tiến độ", details = ex.Message });
            }
        }

        [HttpPost("update-lesson")]
        public async Task<ActionResult<ProgressDto>> UpdateLessonProgress([FromQuery] long courseId, [FromBody] UpdateProgressDto updateProgressDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                var progress = await _progressService.UpdateLessonProgressAsync(userId, courseId, updateProgressDto);
                return Ok(progress);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật tiến độ bài học", details = ex.Message });
            }
        }
    }
}
