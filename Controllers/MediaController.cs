using be_net.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace be_net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<MediaController> _logger;

        public MediaController(IWebHostEnvironment environment, ILogger<MediaController> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<ActionResult<MediaUploadResultDto>> UploadMedia(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                // Validate file size (max 10MB)
                if (file.Length > 10 * 1024 * 1024)
                    return BadRequest("File size exceeds the limit (10MB)");

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".pdf" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest("File type not allowed. Allowed types: jpg, jpeg, png, gif, mp4, pdf");

                // Create uploads directory if it doesn't exist
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Generate unique filename
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Generate URL
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var fileUrl = $"{baseUrl}/uploads/{uniqueFileName}";

                return Ok(new MediaUploadResultDto
                {
                    Url = fileUrl,
                    Message = "File uploaded successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return StatusCode(500, "An error occurred while uploading the file. Please try again later.");
            }
        }

        [HttpPost("users/uploads")]
        [Authorize]
        public async Task<ActionResult<MediaUploadResultDto>> UploadUserImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                // Validate file size (max 2MB)
                if (file.Length > 2 * 1024 * 1024)
                    return BadRequest("File size exceeds the limit (2MB)");

                // Validate file type (only images)
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest("File type not allowed. Allowed types: jpg, jpeg, png, gif");

                // Create user uploads directory if it doesn't exist
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "users");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Get user ID for folder organization
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    var userFolder = Path.Combine(uploadsFolder, userId);
                    if (!Directory.Exists(userFolder))
                        Directory.CreateDirectory(userFolder);
                    uploadsFolder = userFolder;
                }

                // Generate unique filename
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Generate URL
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var fileUrl = string.IsNullOrEmpty(userId)
                    ? $"{baseUrl}/uploads/users/{uniqueFileName}"
                    : $"{baseUrl}/uploads/users/{userId}/{uniqueFileName}";

                return Ok(new MediaUploadResultDto
                {
                    Url = fileUrl,
                    Message = "User image uploaded successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading user image");
                return StatusCode(500, "An error occurred while uploading the image. Please try again later.");
            }
        }

        [HttpPost("blogs/uploads")]
        [Authorize(Roles = "admin,instructor")]
        public async Task<ActionResult<MediaUploadResultDto>> UploadBlogImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                // Validate file size (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                    return BadRequest("File size exceeds the limit (5MB)");

                // Validate file type (only images)
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest("File type not allowed. Allowed types: jpg, jpeg, png, gif");

                // Create blog uploads directory if it doesn't exist
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "blogs");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Generate unique filename
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Generate URL
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var fileUrl = $"{baseUrl}/uploads/blogs/{uniqueFileName}";

                return Ok(new MediaUploadResultDto
                {
                    Url = fileUrl,
                    Message = "Blog image uploaded successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading blog image");
                return StatusCode(500, "An error occurred while uploading the image. Please try again later.");
            }
        }

        [HttpPost("courses/uploads")]
        [Authorize(Roles = "admin,instructor")]
        public async Task<ActionResult<MediaUploadResultDto>> UploadCourseImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                // Validate file size (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                    return BadRequest("File size exceeds the limit (5MB)");

                // Validate file type (only images)
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest("File type not allowed. Allowed types: jpg, jpeg, png, gif");

                // Create course uploads directory if it doesn't exist
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "courses", "images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Generate unique filename
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Generate URL
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var fileUrl = $"{baseUrl}/uploads/courses/images/{uniqueFileName}";

                return Ok(new MediaUploadResultDto
                {
                    Url = fileUrl,
                    Message = "Course image uploaded successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading course image");
                return StatusCode(500, "An error occurred while uploading the image. Please try again later.");
            }
        }

        [HttpPost("courses/uploads-videos")]
        [Authorize(Roles = "admin,instructor")]
        public async Task<ActionResult<MediaUploadResultDto>> UploadCourseVideo(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                // Validate file size (max 100MB)
                if (file.Length > 100 * 1024 * 1024)
                    return BadRequest("File size exceeds the limit (100MB)");

                // Validate file type (only videos)
                var allowedExtensions = new[] { ".mp4", ".webm", ".mov" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest("File type not allowed. Allowed types: mp4, webm, mov");

                // Create course uploads directory if it doesn't exist
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "courses", "videos");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Generate unique filename
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Generate URL
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var fileUrl = $"{baseUrl}/uploads/courses/videos/{uniqueFileName}";

                return Ok(new MediaUploadResultDto
                {
                    Url = fileUrl,
                    Message = "Course video uploaded successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading course video");
                return StatusCode(500, "An error occurred while uploading the video. Please try again later.");
            }
        }
    }
}
