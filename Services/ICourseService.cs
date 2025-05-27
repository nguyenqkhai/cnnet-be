using LmsBackend.DTOs;

namespace LmsBackend.Services
{
    public interface ICourseService
    {
        Task<List<CourseDto>> GetAllCoursesAsync();
        Task<CourseDto> GetCourseByIdAsync(long id);
        Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto);
        Task<CourseDto> UpdateCourseAsync(long id, UpdateCourseDto updateCourseDto);
        Task<bool> DeleteCourseAsync(long id);
    }
}
