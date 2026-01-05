using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ElearningBackend.Data;
using ElearningBackend.DTOs;
using ElearningBackend.Models;

namespace ElearningBackend.Services
{
    public class OrderService : IOrderService
    {
        private readonly LmsDbContext _context;
        private readonly IMapper _mapper;

        public OrderService(LmsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<OrderDto>> GetOrdersByUserIdAsync(long userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId && !o.Destroy)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(o => new OrderDto
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
            }).ToList();
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            // Xác minh người dùng và khóa học tồn tại
            var userExists = await _context.Users.AnyAsync(u => u.Id == createOrderDto.UserId && !u.Destroy);
            var courseExists = await _context.Courses.AnyAsync(c => c.Id == createOrderDto.CourseId && !c.Destroy);

            if (!userExists)
                throw new NotFoundException("User not found");
            if (!courseExists)
                throw new NotFoundException("Course not found");

            // Kiểm tra xem người dùng đã có khóa học này chưa (completed) hoặc đang có order pending
            var existingCompletedOrder = await _context.Orders
                .FirstOrDefaultAsync(o => o.UserId == createOrderDto.UserId &&
                                         o.CourseId == createOrderDto.CourseId &&
                                         o.Status.ToLower() == "completed" &&
                                         !o.Destroy);

            if (existingCompletedOrder != null)
            {
                throw new InvalidOperationException("User already owns this course");
            }

            // Kiểm tra xem có order pending nào không, nếu có thì xóa order cũ để tạo order mới
            var existingPendingOrders = await _context.Orders
                .Where(o => o.UserId == createOrderDto.UserId &&
                           o.CourseId == createOrderDto.CourseId &&
                           (o.Status.ToLower() == "pending" || o.Status.ToLower() == "canceled") &&
                           !o.Destroy)
                .ToListAsync();

            if (existingPendingOrders.Any())
            {
                // Xóa tất cả order pending/canceled cũ để cho phép tạo order mới với payment method khác
                foreach (var pendingOrder in existingPendingOrders)
                {
                    pendingOrder.Destroy = true;
                    pendingOrder.UpdatedAt = DateTime.Now;
                }
                Console.WriteLine($"🔍 Removed {existingPendingOrders.Count} existing pending/canceled orders for user {createOrderDto.UserId}, course {createOrderDto.CourseId}");
            }

            var order = new Order
            {
                UserId = createOrderDto.UserId,
                CourseId = createOrderDto.CourseId,
                UserEmail = createOrderDto.UserEmail,
                UserName = createOrderDto.UserName,
                CourseName = createOrderDto.CourseName,
                CourseThumbnail = createOrderDto.CourseThumbnail,
                Instructor = createOrderDto.Instructor,
                TotalPrice = createOrderDto.TotalPrice,
                PaymentMethod = createOrderDto.PaymentMethod,
                Status = "pending",
                CreatedAt = DateTime.Now
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return new OrderDto
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
        }

        public async Task<List<OrderDto>> GetAllOrdersForAdminAsync()
        {
            var orders = await _context.Orders
                .Where(o => !o.Destroy)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(o => new OrderDto
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
            }).ToList();
        }

        public async Task<OrderDto> GetOrderByIdAsync(long id)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && !o.Destroy);

            if (order == null)
            {
                throw new NotFoundException("Order not found");
            }

            return new OrderDto
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
        }

        public async Task<OrderDto> UpdateOrderAsync(long id, UpdateOrderDto updateOrderDto)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && !o.Destroy);

            if (order == null)
            {
                throw new NotFoundException("Order not found");
            }

            if (!string.IsNullOrEmpty(updateOrderDto.Status))
                order.Status = updateOrderDto.Status;
            if (!string.IsNullOrEmpty(updateOrderDto.PaymentMethod))
                order.PaymentMethod = updateOrderDto.PaymentMethod;

            order.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new OrderDto
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
        }

        public async Task<bool> DeleteOrderAsync(long id)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && !o.Destroy);

            if (order == null)
            {
                return false;
            }

            order.Destroy = true;
            order.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
