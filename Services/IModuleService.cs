using LmsBackend.DTOs;

namespace LmsBackend.Services
{
    public interface IModuleService
    {
        Task<List<ModuleDto>> GetAllModulesAsync();
        Task<ModuleDto> CreateModuleAsync(CreateModuleDto createModuleDto);
        Task<List<ModuleDto>> GetModulesByCourseIdAsync(long courseId);
        Task<ModuleDto> UpdateModuleAsync(long id, UpdateModuleDto updateModuleDto);
        Task<bool> DeleteModuleAsync(long id);
    }
}
