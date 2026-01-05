using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ElearningBackend.Data;
using ElearningBackend.DTOs;
using ElearningBackend.Models;

namespace ElearningBackend.Services
{
    public class LessonService : ILessonService
    {
        private readonly LmsDbContext _context;
        private readonly IMapper _mapper;

        public LessonService(LmsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<LessonDto>> GetAllLessonsAsync()
        {
            var lessons = await _context.Lessons
                .Where(l => !l.Destroy)
                .OrderBy(l => l.CreatedAt)
                .ToListAsync();

            return lessons.Select(l => new LessonDto
            {
                Id = l.Id,
                Name = l.Name,
                VideoUrl = l.VideoUrl,
                CourseId = l.CourseId,
                CoursePart = l.CoursePart,
                CreatedAt = l.CreatedAt,
                UpdatedAt = l.UpdatedAt
            }).ToList();
        }

        public async Task<LessonDto> CreateLessonAsync(CreateLessonDto createLessonDto)
        {
            // Xác minh khóa học tồn tại
            var courseExists = await _context.Courses
                .AnyAsync(c => c.Id == createLessonDto.CourseId && !c.Destroy);

            if (!courseExists)
            {
                throw new NotFoundException("Course not found");
            }

            var lesson = new Lesson
            {
                Name = createLessonDto.Name,
                VideoUrl = createLessonDto.VideoUrl,
                CourseId = createLessonDto.CourseId,
                CoursePart = createLessonDto.CoursePart,
                CreatedAt = DateTime.Now
            };

            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            return new LessonDto
            {
                Id = lesson.Id,
                Name = lesson.Name,
                VideoUrl = lesson.VideoUrl,
                CourseId = lesson.CourseId,
                CoursePart = lesson.CoursePart,
                CreatedAt = lesson.CreatedAt,
                UpdatedAt = lesson.UpdatedAt
            };
        }

        public async Task<List<LessonDto>> GetLessonsByCourseIdAsync(long courseId)
        {
            var lessons = await _context.Lessons
                .Where(l => l.CourseId == courseId && !l.Destroy)
                .OrderBy(l => l.CreatedAt)
                .ToListAsync();

            return lessons.Select(l => new LessonDto
            {
                Id = l.Id,
                Name = l.Name,
                VideoUrl = l.VideoUrl,
                CourseId = l.CourseId,
                CoursePart = l.CoursePart,
                CreatedAt = l.CreatedAt,
                UpdatedAt = l.UpdatedAt
            }).ToList();
        }

        public async Task<LessonDto> UpdateLessonAsync(long id, UpdateLessonDto updateLessonDto)
        {
            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(l => l.Id == id && !l.Destroy);

            if (lesson == null)
            {
                throw new NotFoundException("Lesson not found");
            }

            if (!string.IsNullOrEmpty(updateLessonDto.Name))
                lesson.Name = updateLessonDto.Name;
            if (updateLessonDto.VideoUrl != null)
                lesson.VideoUrl = updateLessonDto.VideoUrl;
            if (!string.IsNullOrEmpty(updateLessonDto.CoursePart))
                lesson.CoursePart = updateLessonDto.CoursePart;

            lesson.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new LessonDto
            {
                Id = lesson.Id,
                Name = lesson.Name,
                VideoUrl = lesson.VideoUrl,
                CourseId = lesson.CourseId,
                CoursePart = lesson.CoursePart,
                CreatedAt = lesson.CreatedAt,
                UpdatedAt = lesson.UpdatedAt
            };
        }

        public async Task<bool> DeleteLessonAsync(long id)
        {
            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(l => l.Id == id && !l.Destroy);

            if (lesson == null)
            {
                return false;
            }

            lesson.Destroy = true;
            lesson.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
