using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LmsBackend.DTOs;
using LmsBackend.Services;

namespace LmsBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<VoucherDto>>> GetAllVouchers()
        {
            try
            {
                var vouchers = await _voucherService.GetAllVouchersAsync();
                return Ok(vouchers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách voucher", details = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<VoucherDto>> CreateVoucher([FromBody] CreateVoucherDto createVoucherDto)
        {
            try
            {
                var voucher = await _voucherService.CreateVoucherAsync(createVoucherDto);
                return Ok(voucher);
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
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo voucher", details = ex.Message });
            }
        }

        [HttpGet("find-by-name")]
        public async Task<ActionResult<VoucherDto>> FindVoucherByName([FromQuery] string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return BadRequest(new { message = "Tham số tên là bắt buộc" });
                }

                var voucher = await _voucherService.FindVoucherByNameAsync(name);
                if (voucher == null)
                {
                    return NotFound(new { message = "Không tìm thấy voucher" });
                }

                return Ok(voucher);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tìm kiếm voucher", details = ex.Message });
            }
        }

        [HttpGet("validate")]
        [Authorize]
        public async Task<ActionResult<VoucherDto>> ValidateVoucher([FromQuery] string code, [FromQuery] long courseId, [FromQuery] int orderValue)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    return BadRequest(new { message = "Mã voucher là bắt buộc" });
                }

                var voucher = await _voucherService.ValidateVoucherAsync(code, courseId, orderValue);
                if (voucher == null)
                {
                    return BadRequest(new { message = "Voucher không hợp lệ, đã hết hạn hoặc không áp dụng cho đơn hàng này" });
                }

                return Ok(voucher);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xác thực voucher", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<VoucherDto>> UpdateVoucher(long id, [FromBody] UpdateVoucherDto updateVoucherDto)
        {
            try
            {
                var voucher = await _voucherService.UpdateVoucherAsync(id, updateVoucherDto);
                return Ok(voucher);
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
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật voucher", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteVoucher(long id)
        {
            try
            {
                var result = await _voucherService.DeleteVoucherAsync(id);
                if (result)
                {
                    return Ok(new { message = "Đã xóa voucher thành công" });
                }
                return NotFound(new { message = "Không tìm thấy voucher" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa voucher", details = ex.Message });
            }
        }

        [HttpPost("{id}/use")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<VoucherDto>> UseVoucher(long id)
        {
            try
            {
                var voucher = await _voucherService.UseVoucherAsync(id);
                return Ok(voucher);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi sử dụng voucher", details = ex.Message });
            }
        }
    }
}
