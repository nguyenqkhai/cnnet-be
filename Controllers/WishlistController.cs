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
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpGet]
        public async Task<ActionResult<List<WishlistDto>>> GetWishlist()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                var wishlists = await _wishlistService.GetWishlistsByUserIdAsync(userId);
                return Ok(wishlists);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách yêu thích", details = ex.Message });
            }
        }

        [HttpGet("admin")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<WishlistDto>>> GetAllWishlistsForAdmin()
        {
            try
            {
                var wishlists = await _wishlistService.GetAllWishlistsAsync();
                return Ok(wishlists);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách yêu thích", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<WishlistDto>> AddToWishlist([FromBody] CreateWishlistDto createWishlistDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                // Đảm bảo userId từ token khớp với userId trong request
                createWishlistDto.UserId = userId;

                var wishlist = await _wishlistService.CreateWishlistAsync(createWishlistDto);
                return Ok(wishlist);
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
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi thêm vào danh sách yêu thích", details = ex.Message });
            }
        }

        [HttpPost("add-course")]
        public async Task<ActionResult<WishlistDto>> AddCourseToWishlist([FromBody] AddCourseToWishlistDto addCourseDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                var wishlist = await _wishlistService.AddCourseToWishlistAsync(userId, addCourseDto.CourseId);
                return Ok(wishlist);
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
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi thêm khóa học vào danh sách yêu thích", details = ex.Message });
            }
        }

        [HttpGet("check/{courseId}")]
        public async Task<ActionResult<bool>> CheckInWishlist(long courseId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                var isInWishlist = await _wishlistService.IsInWishlistAsync(userId, courseId);
                return Ok(isInWishlist);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi kiểm tra danh sách yêu thích", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveFromWishlist(long id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                // Nếu không phải admin, kiểm tra quyền sở hữu
                if (userRole != "admin")
                {
                    var findDto = new FindWishlistDto { UserId = userId, CourseId = 0 }; // Sẽ tìm theo id
                    var wishlist = await _wishlistService.FindByUserAndCourseAsync(findDto);
                    if (wishlist == null || wishlist.UserId != userId)
                    {
                        return Forbid("Bạn chỉ có thể xóa khóa học yêu thích của mình");
                    }
                }

                var result = await _wishlistService.DeleteWishlistAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Không tìm thấy mục yêu thích" });
                }

                return Ok(new { message = "Đã xóa khỏi danh sách yêu thích" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa khỏi danh sách yêu thích", details = ex.Message });
            }
        }

        [HttpDelete("course/{courseId}")]
        public async Task<ActionResult> RemoveCourseFromWishlist(long courseId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                var result = await _wishlistService.RemoveFromWishlistAsync(userId, courseId);
                if (!result)
                {
                    return NotFound(new { message = "Khóa học không có trong danh sách yêu thích" });
                }

                return Ok(new { message = "Đã xóa khỏi danh sách yêu thích" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa khỏi danh sách yêu thích", details = ex.Message });
            }
        }

        [HttpPost("find")]
        public async Task<ActionResult<WishlistDto>> FindWishlistItem([FromBody] FindWishlistDto findWishlistDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                // Đảm bảo userId từ token khớp với userId trong request
                findWishlistDto.UserId = userId;

                var wishlist = await _wishlistService.FindByUserAndCourseAsync(findWishlistDto);
                if (wishlist == null)
                {
                    return NotFound(new { message = "Không tìm thấy trong danh sách yêu thích" });
                }

                return Ok(wishlist);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tìm kiếm trong danh sách yêu thích", details = ex.Message });
            }
        }
    }
}
