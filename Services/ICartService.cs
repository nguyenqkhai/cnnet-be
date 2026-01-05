using ElearningBackend.DTOs;

namespace ElearningBackend.Services
{
    public interface ICartService
    {
        Task<List<CartDto>> GetAllCartsAsync();
        Task<CartDto> CreateCartAsync(CreateCartDto createCartDto);
        Task<CartDto?> FindByUserAndCourseAsync(FindCartDto findCartDto);
        Task<CartDto> UpdateCartAsync(long id, UpdateCartDto updateCartDto);
        Task<bool> DeleteCartAsync(long id);
        Task<List<CartDto>> GetCartsByUserIdAsync(long userId);
    }
}
