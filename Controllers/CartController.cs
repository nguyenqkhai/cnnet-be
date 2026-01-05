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
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CartDto>>> GetCart()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                var carts = await _cartService.GetCartsByUserIdAsync(userId);
                return Ok(carts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy giỏ hàng", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<CartDto>> AddToCart([FromBody] CreateCartDto createCartDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                createCartDto.UserId = userId; 

                var cart = await _cartService.CreateCartAsync(createCartDto);
                return Ok(cart);
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
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi thêm sản phẩm vào giỏ hàng", details = ex.Message });
            }
        }

        [HttpGet("find-by-user-and-course")]
        public async Task<ActionResult<CartDto>> FindByUserAndCourse([FromQuery] long courseId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                var findCartDto = new FindCartDto { UserId = userId, CourseId = courseId };
                var cart = await _cartService.FindByUserAndCourseAsync(findCartDto);

                if (cart == null)
                {
                    return NotFound(new { message = "Không tìm thấy sản phẩm trong giỏ hàng" });
                }

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tìm kiếm sản phẩm trong giỏ hàng", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CartDto>> UpdateCart(long id, [FromBody] UpdateCartDto updateCartDto)
        {
            try
            {
                var cart = await _cartService.UpdateCartAsync(id, updateCartDto);
                return Ok(cart);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật sản phẩm trong giỏ hàng", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveFromCart(long id)
        {
            try
            {
                var result = await _cartService.DeleteCartAsync(id);
                if (result)
                {
                    return Ok(new { message = "Đã xóa sản phẩm khỏi giỏ hàng thành công" });
                }
                return NotFound(new { message = "Không tìm thấy sản phẩm trong giỏ hàng" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa sản phẩm khỏi giỏ hàng", details = ex.Message });
            }
        }
    }
}
