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
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly CourseDBContext _context;
        private readonly ILogger<CartController> _logger;

        public CartController(CourseDBContext context, ILogger<CartController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartDto>>> GetCarts()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var carts = await _context.Carts
                    .Where(c => c.UserId == long.Parse(userId) && !c.Destroy)
                    .Select(c => new CartDto
                    {
                        Id = c.Id,
                        UserId = c.UserId,
                        CourseId = c.CourseId,
                        CourseName = c.CourseName,
                        CourseThumbnail = c.CourseThumbnail,
                        Instructor = c.Instructor,
                        Duration = c.Duration,
                        TotalPrice = c.TotalPrice,
                        TotalLessons = c.TotalLessons,
                        TotalReviews = c.TotalReviews,
                        Rating = c.Rating,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(carts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting carts");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpGet("find-by-user-and-course")]
        public async Task<ActionResult<CartDto>> FindCartByUserAndCourse([FromQuery] CartFindDto cartFindDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                // Ensure the user is only accessing their own cart
                if (cartFindDto.UserId != long.Parse(userId) && !User.IsInRole("admin"))
                    return Forbid();

                var cart = await _context.Carts
                    .Where(c => c.UserId == cartFindDto.UserId && c.CourseId == cartFindDto.CourseId && !c.Destroy)
                    .Select(c => new CartDto
                    {
                        Id = c.Id,
                        UserId = c.UserId,
                        CourseId = c.CourseId,
                        CourseName = c.CourseName,
                        CourseThumbnail = c.CourseThumbnail,
                        Instructor = c.Instructor,
                        Duration = c.Duration,
                        TotalPrice = c.TotalPrice,
                        TotalLessons = c.TotalLessons,
                        TotalReviews = c.TotalReviews,
                        Rating = c.Rating,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (cart == null)
                    return NotFound();

                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding cart by user and course");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CartDto>> AddToCart(CartCreateDto cartCreateDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var user = await _context.Users.FindAsync(long.Parse(userId));
                if (user == null || user.Destroy)
                    return NotFound("User not found");

                var course = await _context.Courses.FindAsync(cartCreateDto.CourseId);
                if (course == null || course.Destroy)
                    return NotFound("Course not found");

                // Check if course is already in cart
                var existingCart = await _context.Carts
                    .FirstOrDefaultAsync(c => c.UserId == user.Id && c.CourseId == cartCreateDto.CourseId && !c.Destroy);

                if (existingCart != null)
                    return BadRequest("Course is already in your cart");

                // Check if user already purchased this course
                var existingOrder = await _context.Orders
                    .FirstOrDefaultAsync(o => o.UserId == user.Id && o.CourseId == cartCreateDto.CourseId && 
                                         o.Status == "completed" && !o.Destroy);

                if (existingOrder != null)
                    return BadRequest("You have already purchased this course");

                // Count total lessons
                var totalLessons = await _context.Lessons
                    .CountAsync(l => l.CourseId == course.Id && !l.Destroy);

                // Count total reviews and calculate average rating
                var reviews = await _context.Reviews
                    .Where(r => r.CourseId == course.Id && !r.Destroy)
                    .ToListAsync();

                int totalReviews = reviews.Count;
                int? averageRating = null;
                if (totalReviews > 0)
                {
                    averageRating = (int)Math.Round(reviews.Average(r => r.Rating));
                }

                // Calculate total price (apply discount if available)
                int totalPrice = course.Price;
                if (course.Discount.HasValue && course.Discount > 0)
                {
                    totalPrice = course.Price - (course.Price * course.Discount.Value / 100);
                }

                var cart = new Cart
                {
                    UserId = user.Id,
                    CourseId = course.Id,
                    CourseName = course.Name,
                    CourseThumbnail = course.Thumbnail,
                    Instructor = course.Instructor,
                    Duration = course.Duration,
                    TotalPrice = totalPrice,
                    TotalLessons = totalLessons,
                    TotalReviews = totalReviews,
                    Rating = averageRating,
                    CreatedAt = DateTime.Now
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();

                var cartDto = new CartDto
                {
                    Id = cart.Id,
                    UserId = cart.UserId,
                    CourseId = cart.CourseId,
                    CourseName = cart.CourseName,
                    CourseThumbnail = cart.CourseThumbnail,
                    Instructor = cart.Instructor,
                    Duration = cart.Duration,
                    TotalPrice = cart.TotalPrice,
                    TotalLessons = cart.TotalLessons,
                    TotalReviews = cart.TotalReviews,
                    Rating = cart.Rating,
                    CreatedAt = cart.CreatedAt,
                    UpdatedAt = cart.UpdatedAt
                };

                return CreatedAtAction(nameof(GetCarts), null, cartDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to cart");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CartDto>> UpdateCart(long id, CartUpdateDto cartUpdateDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var cart = await _context.Carts.FindAsync(id);

                if (cart == null || cart.Destroy)
                    return NotFound();

                // Ensure the user is only updating their own cart
                if (cart.UserId != long.Parse(userId) && !User.IsInRole("admin"))
                    return Forbid();

                // Update only the properties that are not null
                if (cartUpdateDto.TotalPrice.HasValue)
                    cart.TotalPrice = cartUpdateDto.TotalPrice.Value;

                cart.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                var cartDto = new CartDto
                {
                    Id = cart.Id,
                    UserId = cart.UserId,
                    CourseId = cart.CourseId,
                    CourseName = cart.CourseName,
                    CourseThumbnail = cart.CourseThumbnail,
                    Instructor = cart.Instructor,
                    Duration = cart.Duration,
                    TotalPrice = cart.TotalPrice,
                    TotalLessons = cart.TotalLessons,
                    TotalReviews = cart.TotalReviews,
                    Rating = cart.Rating,
                    CreatedAt = cart.CreatedAt,
                    UpdatedAt = cart.UpdatedAt
                };

                return Ok(cartDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCart(long id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var cart = await _context.Carts.FindAsync(id);

                if (cart == null)
                    return NotFound();

                // Ensure the user is only deleting their own cart
                if (cart.UserId != long.Parse(userId) && !User.IsInRole("admin"))
                    return Forbid();

                // Soft delete
                cart.Destroy = true;
                cart.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Cart item removed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cart");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }
    }
}
