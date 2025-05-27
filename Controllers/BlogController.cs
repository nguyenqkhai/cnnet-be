using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LmsBackend.DTOs;
using LmsBackend.Services;

namespace LmsBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet]
        public async Task<ActionResult<List<BlogDto>>> GetAllBlogs()
        {
            try
            {
                var blogs = await _blogService.GetAllBlogsAsync();
                return Ok(blogs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách bài viết", details = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin,author")]
        public async Task<ActionResult<BlogDto>> CreateBlog([FromBody] CreateBlogDto createBlogDto)
        {
            try
            {
                var blog = await _blogService.CreateBlogAsync(createBlogDto);
                return Ok(blog);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo bài viết", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BlogDto>> GetBlogById(long id)
        {
            try
            {
                var blog = await _blogService.GetBlogByIdAsync(id);
                return Ok(blog);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = "Không tìm thấy bài viết", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy bài viết", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,author")]
        public async Task<ActionResult<BlogDto>> UpdateBlog(long id, [FromBody] UpdateBlogDto updateBlogDto)
        {
            try
            {
                var blog = await _blogService.UpdateBlogAsync(id, updateBlogDto);
                return Ok(blog);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = "Không tìm thấy bài viết để cập nhật", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật bài viết", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteBlog(long id)
        {
            try
            {
                var result = await _blogService.DeleteBlogAsync(id);
                if (result)
                {
                    return Ok(new { message = "Xóa bài viết thành công" });
                }
                return NotFound(new { message = "Không tìm thấy bài viết cần xóa" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa bài viết", details = ex.Message });
            }
        }
    }
}
