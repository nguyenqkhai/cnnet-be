using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LmsBackend.DTOs;
using LmsBackend.Services;

namespace LmsBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AdminController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("users")]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            try
            {
                var users = await _authService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách người dùng", details = ex.Message });
            }
        }

        [HttpPut("users/{id}/role")]
        public async Task<ActionResult<UserDto>> UpdateUserRole(long id, [FromBody] UpdateUserRoleDto updateRoleDto)
        {
            try
            {
                var user = await _authService.UpdateUserRoleAsync(id, updateRoleDto);
                return Ok(user);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = $"Không tìm thấy người dùng với ID {id} : {ex.Message}"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Đã xảy ra lỗi khi cập nhật vai trò người dùng", details = ex.Message });
            }
        }

        [HttpGet("dashboard/stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            try
            {
                var stats = await _authService.GetDashboardStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Xảy ra lỗi khi lấy thống kê dashboard", details = ex.Message });
            }
        }

        [HttpPost("users/create")]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                var user = await _authService.CreateUserAsync(createUserDto);
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message =$"Không thể tạo người dùng mới: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo người dùng mới", details = ex.Message });
            }
        }

        [HttpDelete("users/{id}")]
        public async Task<ActionResult> DeleteUser(long id)
        {
            try
            {
                var result = await _authService.DeleteUserAsync(id);
                if (result)
                {
                    return Ok(new { message = "Xóa người dùng thành công" });
                }
                return NotFound(new { message = "Không tìm thấy người dùng để xóa" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa người dùng", details = ex.Message });
            }
        }
    }
}
