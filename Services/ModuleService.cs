using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ElearningBackend.Data;
using ElearningBackend.DTOs;
using ElearningBackend.Models;

namespace ElearningBackend.Services
{
    public class ModuleService : IModuleService
    {
        private readonly LmsDbContext _context;
        private readonly IMapper _mapper;

        public ModuleService(LmsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ModuleDto>> GetAllModulesAsync()
        {
            var modules = await _context.Modules
                .Where(m => !m.Destroy)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            return modules.Select(m => new ModuleDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                Duration = m.Duration,
                Lessons = m.Lessons,
                CourseId = m.CourseId,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            }).ToList();
        }

        public async Task<ModuleDto> CreateModuleAsync(CreateModuleDto createModuleDto)
        {
            // Xác minh khóa học tồn tại
            var courseExists = await _context.Courses
                .AnyAsync(c => c.Id == createModuleDto.CourseId && !c.Destroy);

            if (!courseExists)
            {
                throw new NotFoundException("Course not found");
            }

            var module = new Module
            {
                Title = createModuleDto.Title,
                Description = createModuleDto.Description,
                Duration = createModuleDto.Duration,
                Lessons = createModuleDto.Lessons,
                CourseId = createModuleDto.CourseId,
                CreatedAt = DateTime.Now
            };

            _context.Modules.Add(module);
            await _context.SaveChangesAsync();

            return new ModuleDto
            {
                Id = module.Id,
                Title = module.Title,
                Description = module.Description,
                Duration = module.Duration,
                Lessons = module.Lessons,
                CourseId = module.CourseId,
                CreatedAt = module.CreatedAt,
                UpdatedAt = module.UpdatedAt
            };
        }

        public async Task<List<ModuleDto>> GetModulesByCourseIdAsync(long courseId)
        {
            var modules = await _context.Modules
                .Where(m => m.CourseId == courseId && !m.Destroy)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            return modules.Select(m => new ModuleDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                Duration = m.Duration,
                Lessons = m.Lessons,
                CourseId = m.CourseId,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            }).ToList();
        }

        public async Task<ModuleDto> UpdateModuleAsync(long id, UpdateModuleDto updateModuleDto)
        {
            var module = await _context.Modules
                .FirstOrDefaultAsync(m => m.Id == id && !m.Destroy);

            if (module == null)
            {
                throw new NotFoundException("Module not found");
            }

            if (!string.IsNullOrEmpty(updateModuleDto.Title))
                module.Title = updateModuleDto.Title;
            if (updateModuleDto.Description != null)
                module.Description = updateModuleDto.Description;
            if (updateModuleDto.Duration.HasValue)
                module.Duration = updateModuleDto.Duration;
            if (updateModuleDto.Lessons != null)
                module.Lessons = updateModuleDto.Lessons;

            module.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new ModuleDto
            {
                Id = module.Id,
                Title = module.Title,
                Description = module.Description,
                Duration = module.Duration,
                Lessons = module.Lessons,
                CourseId = module.CourseId,
                CreatedAt = module.CreatedAt,
                UpdatedAt = module.UpdatedAt
            };
        }

        public async Task<bool> DeleteModuleAsync(long id)
        {
            var module = await _context.Modules
                .FirstOrDefaultAsync(m => m.Id == id && !m.Destroy);

            if (module == null)
            {
                return false;
            }

            module.Destroy = true;
            module.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
