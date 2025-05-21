using be_net.Models;
using be_net.Models.DTOs;
using be_net.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace be_net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly CourseDBContext _context;
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(CourseDBContext context, IAuthService authService, ILogger<AuthController> logger)
        {
            _context = context;
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
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

                var token = _authService.GenerateJwtToken(user);

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    Avatar = user.Avatar,
                    Role = user.Role,
                    Token = token
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(500, "An error occurred during registration. Please try again later.");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            try
            {
                if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                {
                    return BadRequest("Email and password are required");
                }

                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email && !u.Destroy);

                if (user == null)
                    return Unauthorized("Invalid email or password");

                var passwordIsValid = _authService.VerifyPassword(user.Password, loginDto.Password);

                if (!passwordIsValid)
                    return Unauthorized("Invalid email or password");

                var token = _authService.GenerateJwtToken(user);

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    Avatar = user.Avatar,
                    Role = user.Role,
                    Token = token
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, "An error occurred during login. Please try again later.");
            }
        }

        [HttpGet("current")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var user = await _context.Users.FindAsync(long.Parse(userId));

                if (user == null || user.Destroy)
                    return NotFound();

                var token = _authService.GenerateJwtToken(user);

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    Avatar = user.Avatar,
                    Role = user.Role,
                    Token = token
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        // [HttpPost("reset-password")]
        // [Authorize]
        // public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        // {
        //     try
        //     {
        //         var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
        //         if (string.IsNullOrEmpty(userId))
        //             return Unauthorized();

        //         var user = await _context.Users.FindAsync(long.Parse(userId));

        //         if (user == null || user.Destroy)
        //             return NotFound();

        //         if (!_authService.VerifyPassword(user.Password, resetPasswordDto.CurrentPassword))
        //             return BadRequest("Current password is incorrect");

        //         user.Password = _authService.HashPassword(resetPasswordDto.NewPassword);
        //         user.UpdatedAt = DateTime.Now;

        //         await _context.SaveChangesAsync();

        //         return Ok(new { message = "Password updated successfully" });
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error resetting password");
        //         return StatusCode(500, "An error occurred. Please try again later.");
        //     }
        // }
    }
}