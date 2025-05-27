using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LmsBackend.DTOs;
using LmsBackend.Services;

namespace LmsBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MediaController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;

        public MediaController(ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("upload")]
        public async Task<ActionResult<UploadResponseDto>> UploadFile(IFormFile file, [FromQuery] string folder = "general")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "Không có file được tải lên" });
                }

                var result = await _cloudinaryService.UploadImageAsync(file, folder);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Tải lên thất bại", details = ex.Message });
            }
        }

        [HttpPost("users/uploads")]
        public async Task<ActionResult<UploadResponseDto>> UploadUserImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "Không có file được tải lên" });
                }

                var result = await _cloudinaryService.UploadUserImageAsync(file);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Tải lên thất bại", details = ex.Message });
            }
        }

        [HttpPost("blogs/uploads")]
        public async Task<ActionResult<UploadResponseDto>> UploadBlogImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "Không có file được tải lên" });
                }

                var result = await _cloudinaryService.UploadBlogImageAsync(file);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Tải lên thất bại", details = ex.Message });
            }
        }

        [HttpPost("courses/uploads")]
        public async Task<ActionResult<UploadResponseDto>> UploadCourseImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "Không có file được tải lên" });
                }

                var result = await _cloudinaryService.UploadCourseImageAsync(file);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Tải lên thất bại", details = ex.Message });
            }
        }

        [HttpPost("courses/uploads-videos")]
        public async Task<ActionResult<UploadResponseDto>> UploadCourseVideo(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "Không có file được tải lên" });
                }

                var result = await _cloudinaryService.UploadCourseVideoAsync(file);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Tải lên thất bại", details = ex.Message });
            }
        }
    }
}
