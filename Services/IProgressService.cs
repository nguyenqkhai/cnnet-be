using LmsBackend.DTOs;

namespace LmsBackend.Services
{
    public interface IProgressService
    {
        Task<List<ProgressDto>> GetProgressByUserIdAsync(long userId);
        Task<ProgressDto?> GetProgressByCourseIdAsync(long userId, long courseId);
        Task<ProgressDto> InitializeProgressAsync(InitProgressDto initProgressDto);
        Task<ProgressDto> UpdateLessonProgressAsync(long userId, long courseId, UpdateProgressDto updateProgressDto);
    }
}
