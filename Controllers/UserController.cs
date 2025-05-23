using be_net.Models;
using be_net.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace be_net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly CourseDBContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(CourseDBContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<UserProfileDto>> GetUserProfile()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var user = await _context.Users.FindAsync(long.Parse(userId));
                if (user == null || user.Destroy)
                    return NotFound();

                var userProfileDto = new UserProfileDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    Avatar = user.Avatar,
                    Role = user.Role
                };

                return Ok(userProfileDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserProfileDto>> UpdateUserProfile(long id, UserProfileUpdateDto userProfileUpdateDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                // Ensure the user is only updating their own profile
                if (id != long.Parse(userId) && !User.IsInRole("admin"))
                    return Forbid();

                var user = await _context.Users.FindAsync(id);
                if (user == null || user.Destroy)
                    return NotFound();

                // Update only the properties that are not null
                if (userProfileUpdateDto.Username != null)
                {
                    // Check if username is already taken
                    var existingUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.Username == userProfileUpdateDto.Username && u.Id != id && !u.Destroy);

                    if (existingUser != null)
                        return BadRequest("Username is already taken");

                    user.Username = userProfileUpdateDto.Username;
                }

                if (userProfileUpdateDto.Avatar != null)
                    user.Avatar = userProfileUpdateDto.Avatar;

                user.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                var userProfileDto = new UserProfileDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    Avatar = user.Avatar,
                    Role = user.Role
                };

                return Ok(userProfileDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }
    }
}
