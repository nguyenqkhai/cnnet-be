using ElearningBackend.DTOs;

namespace ElearningBackend.Services
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto> UpdateUserAsync(long id, UpdateUserDto updateUserDto);
    }
}
