using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LmsBackend.DTOs;
using LmsBackend.Services;

namespace LmsBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<ReviewDto>>> GetAllReviews()
        {
            try
            {
                var reviews = await _reviewService.GetAllReviewsAsync();
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách đánh giá", details = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReviewDto>> CreateReview([FromBody] CreateReviewDto createReviewDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                createReviewDto.UserId = userId; 

                var review = await _reviewService.CreateReviewAsync(createReviewDto);
                return Ok(review);
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
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo đánh giá", details = ex.Message });
            }
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<List<ReviewDto>>> GetReviewsByCourseId(long courseId)
        {
            try
            {
                var reviews = await _reviewService.GetReviewsByCourseIdAsync(courseId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách đánh giá", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ReviewDto>> UpdateReview(long id, [FromBody] UpdateReviewDto updateReviewDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                // Check if user owns this review or is admin
                var existingReview = await _reviewService.GetAllReviewsAsync();
                var review = existingReview.FirstOrDefault(r => r.Id == id);

                if (review == null)
                {
                    return NotFound(new { message = "Không tìm thấy đánh giá" });
                }

                if (userRole != "admin" && review.UserId != userId)
                {
                    return Forbid("Bạn chỉ có thể cập nhật đánh giá của mình");
                }

                var updatedReview = await _reviewService.UpdateReviewAsync(id, updateReviewDto);
                return Ok(updatedReview);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật đánh giá", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteReview(long id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                var existingReviews = await _reviewService.GetAllReviewsAsync();
                var review = existingReviews.FirstOrDefault(r => r.Id == id);

                if (review == null)
                {
                    return NotFound(new { message = "Không tìm thấy đánh giá" });
                }

                if (userRole != "admin" && review.UserId != userId)
                {
                    return Forbid("Bạn chỉ có thể xóa đánh giá của mình");
                }

                var result = await _reviewService.DeleteReviewAsync(id);
                if (result)
                {
                    return Ok(new { message = "Đã xóa đánh giá thành công" });
                }
                return NotFound(new { message = "Không tìm thấy đánh giá" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa đánh giá", details = ex.Message });
            }
        }
    }
}
