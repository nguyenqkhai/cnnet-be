using ElearningBackend.DTOs;

namespace ElearningBackend.Services
{
    public interface ILessonService
    {
        Task<List<LessonDto>> GetAllLessonsAsync();
        Task<LessonDto> CreateLessonAsync(CreateLessonDto createLessonDto);
        Task<List<LessonDto>> GetLessonsByCourseIdAsync(long courseId);
        Task<LessonDto> UpdateLessonAsync(long id, UpdateLessonDto updateLessonDto);
        Task<bool> DeleteLessonAsync(long id);
    }
}
