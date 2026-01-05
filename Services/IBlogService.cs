using ElearningBackend.DTOs;

namespace ElearningBackend.Services
{
    public interface IBlogService
    {
        Task<List<BlogDto>> GetAllBlogsAsync();
        Task<BlogDto> GetBlogByIdAsync(long id);
        Task<BlogDto> CreateBlogAsync(CreateBlogDto createBlogDto);
        Task<BlogDto> UpdateBlogAsync(long id, UpdateBlogDto updateBlogDto);
        Task<bool> DeleteBlogAsync(long id);
    }
}
