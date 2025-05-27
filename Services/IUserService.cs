using LmsBackend.DTOs;

namespace LmsBackend.Services
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto> UpdateUserAsync(long id, UpdateUserDto updateUserDto);
    }
}
