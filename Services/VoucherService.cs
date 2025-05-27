using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using LmsBackend.Data;
using LmsBackend.DTOs;
using LmsBackend.Models;

namespace LmsBackend.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly LmsDbContext _context;
        private readonly IMapper _mapper;

        public VoucherService(LmsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<VoucherDto>> GetAllVouchersAsync()
        {
            var vouchers = await _context.Vouchers
                .Where(v => !v.Destroy)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();

            return vouchers.Select(v => new VoucherDto
            {
                Id = v.Id,
                Name = v.Name,
                Code = v.Code,
                Discount = v.Discount,
                CourseIds = GetSafeCourseIds(v.CourseIdsJson),
                UsageLimit = v.UsageLimit,
                UsedCount = v.UsedCount,
                MinOrderValue = v.MinOrderValue,
                ExpiredAt = v.ExpiredAt,
                CreatedAt = v.CreatedAt,
                UpdatedAt = v.UpdatedAt
            }).ToList();
        }

        private List<long> GetSafeCourseIds(string? courseIdsJson)
        {
            if (string.IsNullOrEmpty(courseIdsJson))
                return new List<long>();

            try
            {
                return JsonConvert.DeserializeObject<List<long>>(courseIdsJson) ?? new List<long>();
            }
            catch
            {
                try
                {
                    var stringIds = JsonConvert.DeserializeObject<List<string>>(courseIdsJson);
                    if (stringIds != null)
                    {
                        var longIds = new List<long>();
                        foreach (var stringId in stringIds)
                        {
                            if (long.TryParse(stringId, out long longId))
                            {
                                longIds.Add(longId);
                            }
                        }
                        return longIds;
                    }
                }
                catch
                {
                }
                return new List<long>();
            }
        }

        public async Task<VoucherDto> CreateVoucherAsync(CreateVoucherDto createVoucherDto)
        {
            var existingVoucher = await _context.Vouchers
                .FirstOrDefaultAsync(v => v.Code.ToLower() == createVoucherDto.Code.ToLower() && !v.Destroy);

            if (existingVoucher != null)
            {
                throw new InvalidOperationException("Voucher code already exists");
            }
            var validCourseIds = createVoucherDto.CourseIds.Where(id => id > 0).ToList();
            if (validCourseIds.Any())
            {
                var existingCourseIds = await _context.Courses
                    .Where(c => validCourseIds.Contains(c.Id) && !c.Destroy)
                    .Select(c => c.Id)
                    .ToListAsync();

                if (existingCourseIds.Count != validCourseIds.Count)
                {
                    throw new NotFoundException("One or more course IDs are invalid");
                }
            }
            var cleanCourseIds = createVoucherDto.CourseIds.Where(id => id > 0).ToList();

            var voucher = new Voucher
            {
                Name = createVoucherDto.Name,
                Code = createVoucherDto.Code.ToUpper(),
                Discount = createVoucherDto.Discount,
                CourseIds = createVoucherDto.CourseIds,
                UsageLimit = createVoucherDto.UsageLimit,
                UsedCount = 0,
                MinOrderValue = createVoucherDto.MinOrderValue,
                ExpiredAt = createVoucherDto.ExpiredAt,
                CreatedAt = DateTime.Now
            };

            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();

            return new VoucherDto
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
        }

        public async Task<VoucherDto?> FindVoucherByNameAsync(string name)
        {
            var voucher = await _context.Vouchers
                .FirstOrDefaultAsync(v => v.Name.ToLower().Contains(name.ToLower()) && !v.Destroy);

            if (voucher == null)
                return null;

            return new VoucherDto
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
        }

        public async Task<VoucherDto> UpdateVoucherAsync(long id, UpdateVoucherDto updateVoucherDto)
        {
            var voucher = await _context.Vouchers
                .FirstOrDefaultAsync(v => v.Id == id && !v.Destroy);

            if (voucher == null)
            {
                throw new NotFoundException("Voucher not found");
            }
            if (!string.IsNullOrEmpty(updateVoucherDto.Code) &&
                updateVoucherDto.Code.ToUpper() != voucher.Code)
            {
                var existingVoucher = await _context.Vouchers
                    .FirstOrDefaultAsync(v => v.Code.ToLower() == updateVoucherDto.Code.ToLower() &&
                                             v.Id != id && !v.Destroy);

                if (existingVoucher != null)
                {
                    throw new InvalidOperationException("Voucher code already exists");
                }
            }
            if (updateVoucherDto.CourseIds != null && updateVoucherDto.CourseIds.Any())
            {
                var validCourseIds = await _context.Courses
                    .Where(c => updateVoucherDto.CourseIds.Contains(c.Id) && !c.Destroy)
                    .Select(c => c.Id)
                    .ToListAsync();

                if (validCourseIds.Count != updateVoucherDto.CourseIds.Count)
                {
                    throw new NotFoundException("One or more course IDs are invalid");
                }
            }
            if (!string.IsNullOrEmpty(updateVoucherDto.Name))
                voucher.Name = updateVoucherDto.Name;
            if (!string.IsNullOrEmpty(updateVoucherDto.Code))
                voucher.Code = updateVoucherDto.Code.ToUpper();
            if (updateVoucherDto.Discount.HasValue)
                voucher.Discount = updateVoucherDto.Discount.Value;
            if (updateVoucherDto.CourseIds != null)
                voucher.CourseIds = updateVoucherDto.CourseIds;
            if (updateVoucherDto.UsageLimit.HasValue)
                voucher.UsageLimit = updateVoucherDto.UsageLimit;
            if (updateVoucherDto.MinOrderValue.HasValue)
                voucher.MinOrderValue = updateVoucherDto.MinOrderValue;
            if (updateVoucherDto.ExpiredAt.HasValue)
                voucher.ExpiredAt = updateVoucherDto.ExpiredAt;

            voucher.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new VoucherDto
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
        }

        public async Task<bool> DeleteVoucherAsync(long id)
        {
            var voucher = await _context.Vouchers
                .FirstOrDefaultAsync(v => v.Id == id && !v.Destroy);

            if (voucher == null)
            {
                return false;
            }

            voucher.Destroy = true;
            voucher.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<VoucherDto?> ValidateVoucherAsync(string code, long courseId, int orderValue)
        {
            var voucher = await _context.Vouchers
                .FirstOrDefaultAsync(v => v.Code.ToUpper() == code.ToUpper() && !v.Destroy);

            if (voucher == null)
                return null;

            // Kiểm tra xem voucher đã hết hạn chưa
            if (voucher.ExpiredAt.HasValue && voucher.ExpiredAt.Value < DateTime.Now)
                return null;

            // Kiểm tra giới hạn sử dụng
            if (voucher.UsageLimit.HasValue && voucher.UsedCount >= voucher.UsageLimit.Value)
                return null;

            // Kiểm tra giá trị đơn hàng tối thiểu
            if (voucher.MinOrderValue.HasValue && orderValue < voucher.MinOrderValue.Value)
                return null;

            // Kiểm tra xem voucher có áp dụng cho khóa học này không (nếu dành riêng cho khóa học)
            if (voucher.CourseIds.Any() && !voucher.CourseIds.Contains(courseId))
                return null;

            return new VoucherDto
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
        }

        public async Task<VoucherDto> UseVoucherAsync(long voucherId)
        {
            var voucher = await _context.Vouchers
                .FirstOrDefaultAsync(v => v.Id == voucherId && !v.Destroy);

            if (voucher == null)
            {
                throw new NotFoundException("Voucher not found");
            }

            voucher.UsedCount++;
            voucher.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new VoucherDto
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
        }
    }
}
