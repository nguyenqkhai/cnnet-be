using LmsBackend.DTOs;

namespace LmsBackend.Services
{
    public interface IReviewService
    {
        Task<List<ReviewDto>> GetAllReviewsAsync();
        Task<ReviewDto> CreateReviewAsync(CreateReviewDto createReviewDto);
        Task<List<ReviewDto>> GetReviewsByCourseIdAsync(long courseId);
        Task<ReviewDto> UpdateReviewAsync(long id, UpdateReviewDto updateReviewDto);
        Task<bool> DeleteReviewAsync(long id);
    }
}
