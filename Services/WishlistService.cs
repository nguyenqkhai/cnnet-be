using Microsoft.EntityFrameworkCore;
using AutoMapper;
using LmsBackend.Data;
using LmsBackend.DTOs;
using LmsBackend.Models;
using LmsBackend.Services;

namespace LmsBackend.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly LmsDbContext _context;
        private readonly IMapper _mapper;

        public WishlistService(LmsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<WishlistDto>> GetAllWishlistsAsync()
        {
            var wishlists = await _context.Wishlists
                .Where(w => !w.Destroy)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();

            return wishlists.Select(w => new WishlistDto
            {
                Id = w.Id,
                UserId = w.UserId,
                CourseId = w.CourseId,
                CourseName = w.CourseName,
                CourseThumbnail = w.CourseThumbnail,
                Instructor = w.Instructor,
                Duration = w.Duration,
                TotalPrice = w.TotalPrice,
                TotalLessons = w.TotalLessons,
                TotalReviews = w.TotalReviews,
                Rating = w.Rating,
                CreatedAt = w.CreatedAt,
                UpdatedAt = w.UpdatedAt
            }).ToList();
        }

        public async Task<List<WishlistDto>> GetWishlistsByUserIdAsync(long userId)
        {
            var wishlists = await _context.Wishlists
                .Where(w => w.UserId == userId && !w.Destroy)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();

            return wishlists.Select(w => new WishlistDto
            {
                Id = w.Id,
                UserId = w.UserId,
                CourseId = w.CourseId,
                CourseName = w.CourseName,
                CourseThumbnail = w.CourseThumbnail,
                Instructor = w.Instructor,
                Duration = w.Duration,
                TotalPrice = w.TotalPrice,
                TotalLessons = w.TotalLessons,
                TotalReviews = w.TotalReviews,
                Rating = w.Rating,
                CreatedAt = w.CreatedAt,
                UpdatedAt = w.UpdatedAt
            }).ToList();
        }

        public async Task<WishlistDto> CreateWishlistAsync(CreateWishlistDto createWishlistDto)
        {
            // Kiểm tra xem sản phẩm đã tồn tại trong wishlist chưa
            var existingWishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == createWishlistDto.UserId &&
                                         w.CourseId == createWishlistDto.CourseId &&
                                         !w.Destroy);

            if (existingWishlist != null)
            {
                throw new InvalidOperationException("Course already exists in wishlist");
            }

            // Xác minh người dùng và khóa học tồn tại
            var userExists = await _context.Users.AnyAsync(u => u.Id == createWishlistDto.UserId && !u.Destroy);
            var courseExists = await _context.Courses.AnyAsync(c => c.Id == createWishlistDto.CourseId && !c.Destroy);

            if (!userExists)
                throw new NotFoundException("User not found");
            if (!courseExists)
                throw new NotFoundException("Course not found");

            var wishlist = new Wishlist
            {
                UserId = createWishlistDto.UserId,
                CourseId = createWishlistDto.CourseId,
                CourseName = createWishlistDto.CourseName,
                CourseThumbnail = createWishlistDto.CourseThumbnail,
                Instructor = createWishlistDto.Instructor,
                Duration = createWishlistDto.Duration,
                TotalPrice = createWishlistDto.TotalPrice,
                TotalLessons = createWishlistDto.TotalLessons,
                TotalReviews = createWishlistDto.TotalReviews,
                Rating = createWishlistDto.Rating,
                CreatedAt = DateTime.Now
            };

            _context.Wishlists.Add(wishlist);
            await _context.SaveChangesAsync();

            return new WishlistDto
            {
                Id = wishlist.Id,
                UserId = wishlist.UserId,
                CourseId = wishlist.CourseId,
                CourseName = wishlist.CourseName,
                CourseThumbnail = wishlist.CourseThumbnail,
                Instructor = wishlist.Instructor,
                Duration = wishlist.Duration,
                TotalPrice = wishlist.TotalPrice,
                TotalLessons = wishlist.TotalLessons,
                TotalReviews = wishlist.TotalReviews,
                Rating = wishlist.Rating,
                CreatedAt = wishlist.CreatedAt,
                UpdatedAt = wishlist.UpdatedAt
            };
        }

        public async Task<WishlistDto?> FindByUserAndCourseAsync(FindWishlistDto findWishlistDto)
        {
            var wishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == findWishlistDto.UserId &&
                                         w.CourseId == findWishlistDto.CourseId &&
                                         !w.Destroy);

            if (wishlist == null)
                return null;

            return new WishlistDto
            {
                Id = wishlist.Id,
                UserId = wishlist.UserId,
                CourseId = wishlist.CourseId,
                CourseName = wishlist.CourseName,
                CourseThumbnail = wishlist.CourseThumbnail,
                Instructor = wishlist.Instructor,
                Duration = wishlist.Duration,
                TotalPrice = wishlist.TotalPrice,
                TotalLessons = wishlist.TotalLessons,
                TotalReviews = wishlist.TotalReviews,
                Rating = wishlist.Rating,
                CreatedAt = wishlist.CreatedAt,
                UpdatedAt = wishlist.UpdatedAt
            };
        }

        public async Task<bool> IsInWishlistAsync(long userId, long courseId)
        {
            return await _context.Wishlists
                .AnyAsync(w => w.UserId == userId && w.CourseId == courseId && !w.Destroy);
        }

        public async Task<bool> DeleteWishlistAsync(long id)
        {
            var wishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.Id == id && !w.Destroy);

            if (wishlist == null)
            {
                return false;
            }

            wishlist.Destroy = true;
            wishlist.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromWishlistAsync(long userId, long courseId)
        {
            var wishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.CourseId == courseId && !w.Destroy);

            if (wishlist == null)
            {
                return false;
            }

            wishlist.Destroy = true;
            wishlist.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
