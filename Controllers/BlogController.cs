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
    public class BlogController : ControllerBase
    {
        private readonly CourseDBContext _context;
        private readonly ILogger<BlogController> _logger;

        public BlogController(CourseDBContext context, ILogger<BlogController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogDto>>> GetBlogs()
        {
            try
            {
                var blogs = await _context.Blogs
                    .Where(b => !b.Destroy)
                    .Select(b => new BlogDto
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Summary = b.Summary,
                        Content = b.Content,
                        Tags = b.Tags,
                        CoverImage = b.CoverImage,
                        Author = b.Author,
                        AuthorId = b.AuthorId,
                        CreatedAt = b.CreatedAt,
                        UpdatedAt = b.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(blogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting blogs");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BlogDto>> GetBlogById(long id)
        {
            try
            {
                var blog = await _context.Blogs
                    .Where(b => b.Id == id && !b.Destroy)
                    .Select(b => new BlogDto
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Summary = b.Summary,
                        Content = b.Content,
                        Tags = b.Tags,
                        CoverImage = b.CoverImage,
                        Author = b.Author,
                        AuthorId = b.AuthorId,
                        CreatedAt = b.CreatedAt,
                        UpdatedAt = b.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (blog == null)
                    return NotFound();

                return Ok(blog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting blog by id");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin,instructor")]
        public async Task<ActionResult<BlogDto>> CreateBlog(BlogCreateDto blogCreateDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var user = await _context.Users.FindAsync(long.Parse(userId));
                if (user == null || user.Destroy)
                    return NotFound("User not found");

                var blog = new Blog
                {
                    Title = blogCreateDto.Title,
                    Summary = blogCreateDto.Summary,
                    Content = blogCreateDto.Content,
                    Tags = blogCreateDto.Tags,
                    CoverImage = blogCreateDto.CoverImage,
                    Author = user.Username,
                    AuthorId = user.Id,
                    CreatedAt = DateTime.Now
                };

                _context.Blogs.Add(blog);
                await _context.SaveChangesAsync();

                var blogDto = new BlogDto
                {
                    Id = blog.Id,
                    Title = blog.Title,
                    Summary = blog.Summary,
                    Content = blog.Content,
                    Tags = blog.Tags,
                    CoverImage = blog.CoverImage,
                    Author = blog.Author,
                    AuthorId = blog.AuthorId,
                    CreatedAt = blog.CreatedAt,
                    UpdatedAt = blog.UpdatedAt
                };

                return CreatedAtAction(nameof(GetBlogById), new { id = blog.Id }, blogDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating blog");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,instructor")]
        public async Task<ActionResult<BlogDto>> UpdateBlog(long id, BlogUpdateDto blogUpdateDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var blog = await _context.Blogs.FindAsync(id);

                if (blog == null || blog.Destroy)
                    return NotFound();

                // Check if the user is the author of the blog or an admin
                if (blog.AuthorId != long.Parse(userId) && !User.IsInRole("admin"))
                    return Forbid();

                // Update only the properties that are not null
                if (blogUpdateDto.Title != null)
                    blog.Title = blogUpdateDto.Title;
                if (blogUpdateDto.Summary != null)
                    blog.Summary = blogUpdateDto.Summary;
                if (blogUpdateDto.Content != null)
                    blog.Content = blogUpdateDto.Content;
                if (blogUpdateDto.Tags != null)
                    blog.Tags = blogUpdateDto.Tags;
                if (blogUpdateDto.CoverImage != null)
                    blog.CoverImage = blogUpdateDto.CoverImage;

                blog.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                var blogDto = new BlogDto
                {
                    Id = blog.Id,
                    Title = blog.Title,
                    Summary = blog.Summary,
                    Content = blog.Content,
                    Tags = blog.Tags,
                    CoverImage = blog.CoverImage,
                    Author = blog.Author,
                    AuthorId = blog.AuthorId,
                    CreatedAt = blog.CreatedAt,
                    UpdatedAt = blog.UpdatedAt
                };

                return Ok(blogDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating blog");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,instructor")]
        public async Task<ActionResult> DeleteBlog(long id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var blog = await _context.Blogs.FindAsync(id);

                if (blog == null)
                    return NotFound();

                // Check if the user is the author of the blog or an admin
                if (blog.AuthorId != long.Parse(userId) && !User.IsInRole("admin"))
                    return Forbid();

                // Soft delete
                blog.Destroy = true;
                blog.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Blog deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting blog");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }
    }
}
