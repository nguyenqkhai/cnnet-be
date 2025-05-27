using AutoMapper;
using Microsoft.EntityFrameworkCore;
using LmsBackend.Data;
using LmsBackend.DTOs;
using LmsBackend.Models;

namespace LmsBackend.Services
{
    public class CartService : ICartService
    {
        private readonly LmsDbContext _context;
        private readonly IMapper _mapper;

        public CartService(LmsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CartDto>> GetAllCartsAsync()
        {
            var carts = await _context.Carts
                .Where(c => !c.Destroy)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return carts.Select(c => new CartDto
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
            }).ToList();
        }

        public async Task<CartDto> CreateCartAsync(CreateCartDto createCartDto)
        {
            // Kiểm tra xem sản phẩm đã tồn tại trong giỏ hàng chưa
            var existingCart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == createCartDto.UserId &&
                                         c.CourseId == createCartDto.CourseId &&
                                         !c.Destroy);

            if (existingCart != null)
            {
                throw new InvalidOperationException("Course already exists in cart");
            }

            // Xác minh người dùng và khóa học tồn tại
            var userExists = await _context.Users.AnyAsync(u => u.Id == createCartDto.UserId && !u.Destroy);
            var courseExists = await _context.Courses.AnyAsync(c => c.Id == createCartDto.CourseId && !c.Destroy);

            if (!userExists)
                throw new NotFoundException("User not found");
            if (!courseExists)
                throw new NotFoundException("Course not found");

            var cart = new Cart
            {
                UserId = createCartDto.UserId,
                CourseId = createCartDto.CourseId,
                CourseName = createCartDto.CourseName,
                CourseThumbnail = createCartDto.CourseThumbnail,
                Instructor = createCartDto.Instructor,
                Duration = createCartDto.Duration,
                TotalPrice = createCartDto.TotalPrice,
                TotalLessons = createCartDto.TotalLessons,
                TotalReviews = createCartDto.TotalReviews,
                Rating = createCartDto.Rating,
                CreatedAt = DateTime.Now
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            return new CartDto
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
        }

        public async Task<CartDto?> FindByUserAndCourseAsync(FindCartDto findCartDto)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == findCartDto.UserId &&
                                         c.CourseId == findCartDto.CourseId &&
                                         !c.Destroy);

            if (cart == null)
                return null;

            return new CartDto
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
        }

        public async Task<CartDto> UpdateCartAsync(long id, UpdateCartDto updateCartDto)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.Id == id && !c.Destroy);

            if (cart == null)
            {
                throw new NotFoundException("Cart item not found");
            }

            if (!string.IsNullOrEmpty(updateCartDto.CourseName))
                cart.CourseName = updateCartDto.CourseName;
            if (updateCartDto.CourseThumbnail != null)
                cart.CourseThumbnail = updateCartDto.CourseThumbnail;
            if (!string.IsNullOrEmpty(updateCartDto.Instructor))
                cart.Instructor = updateCartDto.Instructor;
            if (updateCartDto.Duration.HasValue)
                cart.Duration = updateCartDto.Duration;
            if (updateCartDto.TotalPrice.HasValue)
                cart.TotalPrice = updateCartDto.TotalPrice.Value;
            if (updateCartDto.TotalLessons.HasValue)
                cart.TotalLessons = updateCartDto.TotalLessons.Value;
            if (updateCartDto.TotalReviews.HasValue)
                cart.TotalReviews = updateCartDto.TotalReviews.Value;
            if (updateCartDto.Rating.HasValue)
                cart.Rating = updateCartDto.Rating;

            cart.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new CartDto
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
        }

        public async Task<bool> DeleteCartAsync(long id)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.Id == id && !c.Destroy);

            if (cart == null)
            {
                return false;
            }

            cart.Destroy = true;
            cart.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CartDto>> GetCartsByUserIdAsync(long userId)
        {
            var carts = await _context.Carts
                .Where(c => c.UserId == userId && !c.Destroy)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return carts.Select(c => new CartDto
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
            }).ToList();
        }
    }
}
