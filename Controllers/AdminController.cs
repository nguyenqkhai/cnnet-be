using be_net.Models;
using be_net.Models.DTOs;
using be_net.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace be_net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "RequireAdminRole")]
    public class AdminController : ControllerBase
    {
        private readonly CourseDBContext _context;
        private readonly IAuthService _authService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(CourseDBContext context, IAuthService authService, ILogger<AdminController> logger)
        {
            _context = context;
            _authService = authService;
            _logger = logger;
        }

        [HttpGet("users")]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            try
            {
                var users = await _context.Users
                    .Where(u => !u.Destroy)
                    .ToListAsync();

                var userDtos = users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Username = u.Username,
                    Avatar = u.Avatar,
                    Role = u.Role,
                    Token = ""
                }).ToList();

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpPut("users/{id}/role")]
        public async Task<ActionResult> UpdateUserRole(long id, [FromBody] string role)
        {
            try
            {
                if (role != "admin" && role != "student" && role != "instructor")
                    return BadRequest("Invalid role. Valid roles are: admin, student, instructor");

                var user = await _context.Users.FindAsync(id);

                if (user == null || user.Destroy)
                    return NotFound();

                user.Role = role;
                user.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "User role updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user role");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpGet("dashboard/stats")]
        public async Task<ActionResult> GetDashboardStats()
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync(u => !u.Destroy);
                var totalCourses = await _context.Courses.CountAsync(c => !c.Destroy);
                var totalOrders = await _context.Orders.CountAsync(o => !o.Destroy);
                var totalRevenue = await _context.Orders
                    .Where(o => o.Status == "completed" && !o.Destroy)
                    .SumAsync(o => o.TotalPrice);

                var stats = new
                {
                    TotalUsers = totalUsers,
                    TotalCourses = totalCourses,
                    TotalOrders = totalOrders,
                    TotalRevenue = totalRevenue
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard stats");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpPost("users/create")]
        public async Task<ActionResult<UserDto>> CreateUser(RegisterDto registerDto)
        {
            try
            {
                if (string.IsNullOrEmpty(registerDto.Email) || string.IsNullOrEmpty(registerDto.Username) || string.IsNullOrEmpty(registerDto.Password))
                {
                    return BadRequest("Email, username and password are required");
                }

                if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                    return BadRequest("Email already in use");

                if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
                    return BadRequest("Username already in use");

                var user = new User
                {
                    Email = registerDto.Email,
                    Username = registerDto.Username,
                    Password = _authService.HashPassword(registerDto.Password),
                    Avatar = registerDto.Avatar,
                    Role = "student", // Default role
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    Avatar = user.Avatar,
                    Role = user.Role,
                    Token = ""
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpDelete("users/{id}")]
        public async Task<ActionResult> DeleteUser(long id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                    return NotFound();

                // Soft delete
                user.Destroy = true;
                user.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }
    }
}