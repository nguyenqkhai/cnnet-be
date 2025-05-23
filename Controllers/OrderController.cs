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
    public class OrderController : ControllerBase
    {
        private readonly CourseDBContext _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(CourseDBContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var orders = await _context.Orders
                    .Where(o => o.UserId == long.Parse(userId) && !o.Destroy)
                    .Select(o => new OrderDto
                    {
                        Id = o.Id,
                        UserId = o.UserId,
                        CourseId = o.CourseId,
                        UserEmail = o.UserEmail,
                        UserName = o.UserName,
                        CourseName = o.CourseName,
                        CourseThumbnail = o.CourseThumbnail,
                        Instructor = o.Instructor,
                        TotalPrice = o.TotalPrice,
                        PaymentMethod = o.PaymentMethod,
                        Status = o.Status,
                        CreatedAt = o.CreatedAt,
                        UpdatedAt = o.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpGet("admin")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            try
            {
                var orders = await _context.Orders
                    .Where(o => !o.Destroy)
                    .Select(o => new OrderDto
                    {
                        Id = o.Id,
                        UserId = o.UserId,
                        CourseId = o.CourseId,
                        UserEmail = o.UserEmail,
                        UserName = o.UserName,
                        CourseName = o.CourseName,
                        CourseThumbnail = o.CourseThumbnail,
                        Instructor = o.Instructor,
                        TotalPrice = o.TotalPrice,
                        PaymentMethod = o.PaymentMethod,
                        Status = o.Status,
                        CreatedAt = o.CreatedAt,
                        UpdatedAt = o.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all orders");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(long id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var order = await _context.Orders
                    .Where(o => o.Id == id && !o.Destroy)
                    .Select(o => new OrderDto
                    {
                        Id = o.Id,
                        UserId = o.UserId,
                        CourseId = o.CourseId,
                        UserEmail = o.UserEmail,
                        UserName = o.UserName,
                        CourseName = o.CourseName,
                        CourseThumbnail = o.CourseThumbnail,
                        Instructor = o.Instructor,
                        TotalPrice = o.TotalPrice,
                        PaymentMethod = o.PaymentMethod,
                        Status = o.Status,
                        CreatedAt = o.CreatedAt,
                        UpdatedAt = o.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (order == null)
                    return NotFound();

                // Check if the user is the owner of the order or an admin
                if (order.UserId != long.Parse(userId) && !User.IsInRole("admin"))
                    return Forbid();

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order by id");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(OrderCreateDto orderCreateDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var user = await _context.Users.FindAsync(long.Parse(userId));
                if (user == null || user.Destroy)
                    return NotFound("User not found");

                var course = await _context.Courses.FindAsync(orderCreateDto.CourseId);
                if (course == null || course.Destroy)
                    return NotFound("Course not found");

                // Check if user already has an order for this course
                var existingOrder = await _context.Orders
                    .FirstOrDefaultAsync(o => o.UserId == user.Id && o.CourseId == orderCreateDto.CourseId && !o.Destroy);

                if (existingOrder != null)
                    return BadRequest("You have already ordered this course");

                // Calculate total price (apply discount if available)
                int totalPrice = course.Price;
                if (course.Discount.HasValue && course.Discount > 0)
                {
                    totalPrice = course.Price - (course.Price * course.Discount.Value / 100);
                }

                var order = new Order
                {
                    UserId = user.Id,
                    CourseId = course.Id,
                    UserEmail = user.Email,
                    UserName = user.Username,
                    CourseName = course.Name,
                    CourseThumbnail = course.Thumbnail,
                    Instructor = course.Instructor,
                    TotalPrice = totalPrice,
                    PaymentMethod = orderCreateDto.PaymentMethod,
                    Status = "pending", // Initial status
                    CreatedAt = DateTime.Now
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                var orderDto = new OrderDto
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    CourseId = order.CourseId,
                    UserEmail = order.UserEmail,
                    UserName = order.UserName,
                    CourseName = order.CourseName,
                    CourseThumbnail = order.CourseThumbnail,
                    Instructor = order.Instructor,
                    TotalPrice = order.TotalPrice,
                    PaymentMethod = order.PaymentMethod,
                    Status = order.Status,
                    CreatedAt = order.CreatedAt,
                    UpdatedAt = order.UpdatedAt
                };

                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, orderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<OrderDto>> UpdateOrder(long id, OrderUpdateDto orderUpdateDto)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);

                if (order == null || order.Destroy)
                    return NotFound();

                // Update only the properties that are not null
                if (orderUpdateDto.Status != null)
                    order.Status = orderUpdateDto.Status;

                order.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                var orderDto = new OrderDto
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    CourseId = order.CourseId,
                    UserEmail = order.UserEmail,
                    UserName = order.UserName,
                    CourseName = order.CourseName,
                    CourseThumbnail = order.CourseThumbnail,
                    Instructor = order.Instructor,
                    TotalPrice = order.TotalPrice,
                    PaymentMethod = order.PaymentMethod,
                    Status = order.Status,
                    CreatedAt = order.CreatedAt,
                    UpdatedAt = order.UpdatedAt
                };

                return Ok(orderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteOrder(long id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);

                if (order == null)
                    return NotFound();

                // Soft delete
                order.Destroy = true;
                order.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Order deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }
    }
}
