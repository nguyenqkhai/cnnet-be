using be_net.Models;
using be_net.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace be_net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly CourseDBContext _context;
        private readonly ILogger<VoucherController> _logger;

        public VoucherController(CourseDBContext context, ILogger<VoucherController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<VoucherDto>>> GetVouchers()
        {
            try
            {
                var vouchers = await _context.Vouchers
                    .Where(v => !v.Destroy)
                    .Select(v => new VoucherDto
                    {
                        Id = v.Id,
                        Name = v.Name,
                        Code = v.Code,
                        Discount = v.Discount,
                        CourseIds = v.CourseIds,
                        UsageLimit = v.UsageLimit,
                        UsedCount = v.UsedCount,
                        MinOrderValue = v.MinOrderValue,
                        ExpiredAt = v.ExpiredAt,
                        CreatedAt = v.CreatedAt,
                        UpdatedAt = v.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(vouchers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vouchers");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpGet("find-by-name")]
        public async Task<ActionResult<VoucherDto>> FindVoucherByName([FromQuery] VoucherFindDto voucherFindDto)
        {
            try
            {
                if (string.IsNullOrEmpty(voucherFindDto.Code))
                    return BadRequest("Voucher code is required");

                var voucher = await _context.Vouchers
                    .Where(v => v.Code == voucherFindDto.Code && !v.Destroy)
                    .FirstOrDefaultAsync();

                if (voucher == null)
                    return NotFound("Voucher not found");

                // Check if voucher is expired
                if (voucher.ExpiredAt.HasValue && voucher.ExpiredAt < DateTime.Now)
                    return BadRequest("Voucher has expired");

                // Check if voucher has reached usage limit
                if (voucher.UsageLimit.HasValue && voucher.UsedCount >= voucher.UsageLimit)
                    return BadRequest("Voucher has reached its usage limit");

                // Check if voucher is applicable to the course
                if (!string.IsNullOrEmpty(voucher.CourseIds))
                {
                    var courseIds = voucher.CourseIds.Split(',').Select(long.Parse).ToList();
                    if (!courseIds.Contains(voucherFindDto.CourseId))
                        return BadRequest("Voucher is not applicable to this course");
                }

                // Get course price to check minimum order value
                var course = await _context.Courses.FindAsync(voucherFindDto.CourseId);
                if (course == null || course.Destroy)
                    return NotFound("Course not found");

                // Check minimum order value
                if (voucher.MinOrderValue.HasValue && course.Price < voucher.MinOrderValue)
                    return BadRequest($"Minimum order value for this voucher is {voucher.MinOrderValue}");

                var voucherDto = new VoucherDto
                {
                    Id = voucher.Id,
                    Name = voucher.Name,
                    Code = voucher.Code,
                    Discount = voucher.Discount,
                    CourseIds = voucher.CourseIds,
                    UsageLimit = voucher.UsageLimit,
                    UsedCount = voucher.UsedCount,
                    MinOrderValue = voucher.MinOrderValue,
                    ExpiredAt = voucher.ExpiredAt,
                    CreatedAt = voucher.CreatedAt,
                    UpdatedAt = voucher.UpdatedAt
                };

                return Ok(voucherDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding voucher by name");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<VoucherDto>> CreateVoucher(VoucherCreateDto voucherCreateDto)
        {
            try
            {
                // Check if voucher code already exists
                var existingVoucher = await _context.Vouchers
                    .FirstOrDefaultAsync(v => v.Code == voucherCreateDto.Code && !v.Destroy);

                if (existingVoucher != null)
                    return BadRequest("Voucher code already exists");

                var voucher = new Voucher
                {
                    Name = voucherCreateDto.Name,
                    Code = voucherCreateDto.Code,
                    Discount = voucherCreateDto.Discount,
                    CourseIds = voucherCreateDto.CourseIds,
                    UsageLimit = voucherCreateDto.UsageLimit,
                    UsedCount = 0,
                    MinOrderValue = voucherCreateDto.MinOrderValue,
                    ExpiredAt = voucherCreateDto.ExpiredAt,
                    CreatedAt = DateTime.Now
                };

                _context.Vouchers.Add(voucher);
                await _context.SaveChangesAsync();

                var voucherDto = new VoucherDto
                {
                    Id = voucher.Id,
                    Name = voucher.Name,
                    Code = voucher.Code,
                    Discount = voucher.Discount,
                    CourseIds = voucher.CourseIds,
                    UsageLimit = voucher.UsageLimit,
                    UsedCount = voucher.UsedCount,
                    MinOrderValue = voucher.MinOrderValue,
                    ExpiredAt = voucher.ExpiredAt,
                    CreatedAt = voucher.CreatedAt,
                    UpdatedAt = voucher.UpdatedAt
                };

                return CreatedAtAction(nameof(GetVouchers), null, voucherDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating voucher");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<VoucherDto>> UpdateVoucher(long id, VoucherUpdateDto voucherUpdateDto)
        {
            try
            {
                var voucher = await _context.Vouchers.FindAsync(id);

                if (voucher == null || voucher.Destroy)
                    return NotFound();

                // Check if voucher code already exists (if updating code)
                if (voucherUpdateDto.Code != null && voucherUpdateDto.Code != voucher.Code)
                {
                    var existingVoucher = await _context.Vouchers
                        .FirstOrDefaultAsync(v => v.Code == voucherUpdateDto.Code && !v.Destroy);

                    if (existingVoucher != null)
                        return BadRequest("Voucher code already exists");
                }

                // Update only the properties that are not null
                if (voucherUpdateDto.Name != null)
                    voucher.Name = voucherUpdateDto.Name;
                if (voucherUpdateDto.Code != null)
                    voucher.Code = voucherUpdateDto.Code;
                if (voucherUpdateDto.Discount.HasValue)
                    voucher.Discount = voucherUpdateDto.Discount.Value;
                if (voucherUpdateDto.CourseIds != null)
                    voucher.CourseIds = voucherUpdateDto.CourseIds;
                if (voucherUpdateDto.UsageLimit.HasValue)
                    voucher.UsageLimit = voucherUpdateDto.UsageLimit;
                if (voucherUpdateDto.MinOrderValue.HasValue)
                    voucher.MinOrderValue = voucherUpdateDto.MinOrderValue;
                if (voucherUpdateDto.ExpiredAt.HasValue)
                    voucher.ExpiredAt = voucherUpdateDto.ExpiredAt;

                voucher.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                var voucherDto = new VoucherDto
                {
                    Id = voucher.Id,
                    Name = voucher.Name,
                    Code = voucher.Code,
                    Discount = voucher.Discount,
                    CourseIds = voucher.CourseIds,
                    UsageLimit = voucher.UsageLimit,
                    UsedCount = voucher.UsedCount,
                    MinOrderValue = voucher.MinOrderValue,
                    ExpiredAt = voucher.ExpiredAt,
                    CreatedAt = voucher.CreatedAt,
                    UpdatedAt = voucher.UpdatedAt
                };

                return Ok(voucherDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating voucher");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteVoucher(long id)
        {
            try
            {
                var voucher = await _context.Vouchers.FindAsync(id);

                if (voucher == null)
                    return NotFound();

                // Soft delete
                voucher.Destroy = true;
                voucher.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Voucher deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting voucher");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }
    }
}
