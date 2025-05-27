using AutoMapper;
using Microsoft.EntityFrameworkCore;
using LmsBackend.Data;
using LmsBackend.DTOs;
using LmsBackend.Models;

namespace LmsBackend.Services
{
    public class ProgressService : IProgressService
    {
        private readonly LmsDbContext _context;
        private readonly IMapper _mapper;

        public ProgressService(LmsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ProgressDto>> GetProgressByUserIdAsync(long userId)
        {
            var progresses = await _context.Progresses
                .Where(p => p.UserId == userId)
                .ToListAsync();

            return progresses.Select(p => new ProgressDto
            {
                Id = p.Id,
                UserId = p.UserId,
                CourseId = p.CourseId,
                CompletedLessons = p.CompletedLessons,
                TotalLessons = p.TotalLessons,
                PercentComplete = p.PercentComplete,
                LastAccessedAt = p.LastAccessedAt
            }).ToList();
        }

        public async Task<ProgressDto?> GetProgressByCourseIdAsync(long userId, long courseId)
        {
            var progress = await _context.Progresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.CourseId == courseId);

            if (progress == null)
                return null;

            return new ProgressDto
            {
                Id = progress.Id,
                UserId = progress.UserId,
                CourseId = progress.CourseId,
                CompletedLessons = progress.CompletedLessons,
                TotalLessons = progress.TotalLessons,
                PercentComplete = progress.PercentComplete,
                LastAccessedAt = progress.LastAccessedAt
            };
        }

        public async Task<ProgressDto> InitializeProgressAsync(InitProgressDto initProgressDto)
        {
            // Xác minh người dùng và khóa học tồn tại
            var userExists = await _context.Users.AnyAsync(u => u.Id == initProgressDto.UserId && !u.Destroy);
            var courseExists = await _context.Courses.AnyAsync(c => c.Id == initProgressDto.CourseId && !c.Destroy);

            if (!userExists)
                throw new NotFoundException("User not found");
            if (!courseExists)
                throw new NotFoundException("Course not found");

            // Kiểm tra xem tiến độ đã tồn tại chưa
            var existingProgress = await _context.Progresses
                .FirstOrDefaultAsync(p => p.UserId == initProgressDto.UserId && p.CourseId == initProgressDto.CourseId);

            if (existingProgress != null)
            {
                return new ProgressDto
                {
                    Id = existingProgress.Id,
                    UserId = existingProgress.UserId,
                    CourseId = existingProgress.CourseId,
                    CompletedLessons = existingProgress.CompletedLessons,
                    TotalLessons = existingProgress.TotalLessons,
                    PercentComplete = existingProgress.PercentComplete,
                    LastAccessedAt = existingProgress.LastAccessedAt
                };
            }

            // Xác minh người dùng đã mua khóa học
            var hasPurchased = await _context.Orders
                .AnyAsync(o => o.UserId == initProgressDto.UserId &&
                              o.CourseId == initProgressDto.CourseId &&
                              o.Status == "completed" &&
                              !o.Destroy);

            if (!hasPurchased)
            {
                throw new InvalidOperationException("User must purchase the course before tracking progress");
            }

            var progress = new Progress
            {
                UserId = initProgressDto.UserId,
                CourseId = initProgressDto.CourseId,
                CompletedLessons = new List<long>(),
                TotalLessons = initProgressDto.TotalLessons,
                PercentComplete = 0,
                LastAccessedAt = DateTime.Now
            };

            _context.Progresses.Add(progress);
            await _context.SaveChangesAsync();

            return new ProgressDto
            {
                Id = progress.Id,
                UserId = progress.UserId,
                CourseId = progress.CourseId,
                CompletedLessons = progress.CompletedLessons,
                TotalLessons = progress.TotalLessons,
                PercentComplete = progress.PercentComplete,
                LastAccessedAt = progress.LastAccessedAt
            };
        }

        public async Task<ProgressDto> UpdateLessonProgressAsync(long userId, long courseId, UpdateProgressDto updateProgressDto)
        {
            var progress = await _context.Progresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.CourseId == courseId);

            if (progress == null)
            {
                throw new NotFoundException("Progress not found. Please initialize progress first.");
            }

            // Xác minh bài học tồn tại và thuộc về khóa học
            var lessonExists = await _context.Lessons
                .AnyAsync(l => l.Id == updateProgressDto.LessonId && l.CourseId == courseId && !l.Destroy);

            if (!lessonExists)
            {
                throw new NotFoundException("Lesson not found in this course");
            }

            var completedLessons = progress.CompletedLessons.ToList();

            if (updateProgressDto.IsCompleted)
            {
                // Thêm bài học vào danh sách hoàn thành nếu chưa có
                if (!completedLessons.Contains(updateProgressDto.LessonId))
                {
                    completedLessons.Add(updateProgressDto.LessonId);
                }
            }
            else
            {
                // Xóa bài học khỏi danh sách hoàn thành
                completedLessons.Remove(updateProgressDto.LessonId);
            }

            progress.CompletedLessons = completedLessons;
            progress.PercentComplete = progress.TotalLessons > 0
                ? (int)Math.Round((double)completedLessons.Count / progress.TotalLessons * 100)
                : 0;
            progress.LastAccessedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new ProgressDto
            {
                Id = progress.Id,
                UserId = progress.UserId,
                CourseId = progress.CourseId,
                CompletedLessons = progress.CompletedLessons,
                TotalLessons = progress.TotalLessons,
                PercentComplete = progress.PercentComplete,
                LastAccessedAt = progress.LastAccessedAt
            };
        }
    }
}
