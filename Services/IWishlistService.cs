using LmsBackend.DTOs;

namespace LmsBackend.Services
{
    public interface IWishlistService
    {
        Task<List<WishlistDto>> GetAllWishlistsAsync();
        Task<WishlistDto> CreateWishlistAsync(CreateWishlistDto createWishlistDto);
        Task<WishlistDto?> FindByUserAndCourseAsync(FindWishlistDto findWishlistDto);
        Task<bool> DeleteWishlistAsync(long id);
        Task<List<WishlistDto>> GetWishlistsByUserIdAsync(long userId);
        Task<bool> IsInWishlistAsync(long userId, long courseId);
        Task<bool> RemoveFromWishlistAsync(long userId, long courseId);
    }
}
