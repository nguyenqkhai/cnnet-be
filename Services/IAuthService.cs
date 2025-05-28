using LmsBackend.DTOs;

namespace LmsBackend.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<bool> LogoutAsync();
        Task<UserDto> GetCurrentUserAsync(long userId);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        string GenerateJwtToken(UserDto user);
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDto> UpdateUserRoleAsync(long userId, UpdateUserRoleDto updateRoleDto);
        Task<bool> DeleteUserAsync(long userId);
        Task<DashboardStatsDto> GetDashboardStatsAsync();
    }
}
