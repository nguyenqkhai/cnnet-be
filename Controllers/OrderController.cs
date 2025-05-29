using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LmsBackend.DTOs;
using LmsBackend.Services;

namespace LmsBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetUserOrders()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                var orders = await _orderService.GetOrdersByUserIdAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách đơn hàng", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                createOrderDto.UserId = userId;

                var order = await _orderService.CreateOrderAsync(createOrderDto);
                return Ok(order);
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
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo đơn hàng", details = ex.Message });
            }
        }

        [HttpGet("admin")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<OrderDto>>> GetAllOrdersForAdmin()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersForAdminAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách đơn hàng", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(long id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                var order = await _orderService.GetOrderByIdAsync(id);

                if (userRole != "admin" && order.UserId != userId)
                {
                    return Forbid("Bạn chỉ có thể truy cập đơn hàng của mình");
                }

                return Ok(order);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<OrderDto>> UpdateOrder(long id, [FromBody] UpdateOrderDto updateOrderDto)
        {
            try
            {
                var order = await _orderService.UpdateOrderAsync(id, updateOrderDto);
                return Ok(order);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật đơn hàng", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteOrder(long id)
        {
            try
            {
                var result = await _orderService.DeleteOrderAsync(id);
                if (result)
                {
                    return Ok(new { message = "Đã xóa đơn hàng thành công" });
                }
                return NotFound(new { message = "Không tìm thấy đơn hàng" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa đơn hàng", details = ex.Message });
            }
        }

        // TEST ENDPOINT - Tạo order completed cho testing
        [HttpPost("test/create-completed")]
        public async Task<ActionResult<OrderDto>> CreateCompletedOrderForTest([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                createOrderDto.UserId = userId;

                // Tạo order và set status thành COMPLETED ngay lập tức
                var order = await _orderService.CreateOrderAsync(createOrderDto);

                // Cập nhật status thành COMPLETED
                await _orderService.UpdateOrderAsync(order.Id, new UpdateOrderDto { Status = "COMPLETED" });

                // Lấy order đã cập nhật
                var completedOrder = await _orderService.GetOrderByIdAsync(order.Id);

                Console.WriteLine($"✅ Created completed order for testing: User {userId}, Course {createOrderDto.CourseId}");

                return Ok(completedOrder);
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
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo đơn hàng test", details = ex.Message });
            }
        }
    }
}
