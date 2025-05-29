using AutoMapper;
using Microsoft.EntityFrameworkCore;
using LmsBackend.Data;
using LmsBackend.DTOs;
using LmsBackend.Models;

namespace LmsBackend.Services
{
    public class CourseService : ICourseService
    {
        private readonly LmsDbContext _context;
        private readonly IMapper _mapper;

        public CourseService(LmsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CourseDto>> GetAllCoursesAsync()
        {
            var courses = await _context.Courses
                .Where(c => !c.Destroy)
                .ToListAsync();

            return courses.Select(c => new CourseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Thumbnail = c.Thumbnail,
                Instructor = c.Instructor,
                InstructorRole = c.InstructorRole,
                InstructorDescription = c.InstructorDescription,
                Duration = c.Duration,
                Price = c.Price,
                Discount = c.Discount,
                Students = c.Students,
                CourseModules = c.CourseModules,
                Category = c.Category,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).ToList();
        }

        public async Task<CourseDto> GetCourseByIdAsync(long id)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == id && !c.Destroy);

            if (course == null)
            {
                throw new NotFoundException("Course not found");
            }

            return new CourseDto
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                Thumbnail = course.Thumbnail,
                Instructor = course.Instructor,
                InstructorRole = course.InstructorRole,
                InstructorDescription = course.InstructorDescription,
                Duration = course.Duration,
                Price = course.Price,
                Discount = course.Discount,
                Students = course.Students,
                CourseModules = course.CourseModules,
                Category = course.Category,
                CreatedAt = course.CreatedAt,
                UpdatedAt = course.UpdatedAt
            };
        }

        public async Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var course = _mapper.Map<Course>(createCourseDto);
                course.CreatedAt = DateTime.Now;

                _context.Courses.Add(course);
                await _context.SaveChangesAsync();

                // Tạo các Module và Lesson từ CourseModules
                if (createCourseDto.CourseModules != null && createCourseDto.CourseModules.Any())
                {
                    foreach (var moduleData in createCourseDto.CourseModules)
                    {
                        // Tạo Module
                        var module = new Module
                        {
                            Title = moduleData.Title,
                            Description = moduleData.Description,
                            Duration = !string.IsNullOrEmpty(moduleData.Duration) && double.TryParse(moduleData.Duration, out var duration) ? duration : null,
                            CourseId = course.Id,
                            CreatedAt = DateTime.Now,
                            Lessons = new List<long>()
                        };

                        _context.Modules.Add(module);
                        await _context.SaveChangesAsync();

                        // Tạo các Lesson cho Module này
                        var lessonIds = new List<long>();
                        if (moduleData.Lessons != null && moduleData.Lessons.Any())
                        {
                            foreach (var lessonData in moduleData.Lessons)
                            {
                                var lesson = new Lesson
                                {
                                    Name = lessonData.Name,
                                    VideoUrl = lessonData.VideoUrl,
                                    CourseId = course.Id,
                                    CoursePart = moduleData.Title, // Sử dụng title của module làm coursePart
                                    CreatedAt = DateTime.Now
                                };

                                _context.Lessons.Add(lesson);
                                await _context.SaveChangesAsync();

                                lessonIds.Add(lesson.Id);
                            }
                        }

                        // Cập nhật danh sách lesson IDs cho module
                        module.Lessons = lessonIds;
                        await _context.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();
                return _mapper.Map<CourseDto>(course);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<CourseDto> UpdateCourseAsync(long id, UpdateCourseDto updateCourseDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var course = await _context.Courses
                    .FirstOrDefaultAsync(c => c.Id == id && !c.Destroy);

                if (course == null)
                {
                    throw new NotFoundException("Course not found");
                }

                _mapper.Map(updateCourseDto, course);
                course.UpdatedAt = DateTime.Now;

                // Nếu có cập nhật CourseModules, cần cập nhật lại Module và Lesson
                if (updateCourseDto.CourseModules != null)
                {
                    // Xóa các Module và Lesson cũ (soft delete)
                    var existingModules = await _context.Modules
                        .Where(m => m.CourseId == id && !m.Destroy)
                        .ToListAsync();

                    var existingLessons = await _context.Lessons
                        .Where(l => l.CourseId == id && !l.Destroy)
                        .ToListAsync();

                    foreach (var module in existingModules)
                    {
                        module.Destroy = true;
                        module.UpdatedAt = DateTime.Now;
                    }

                    foreach (var lesson in existingLessons)
                    {
                        lesson.Destroy = true;
                        lesson.UpdatedAt = DateTime.Now;
                    }

                    // Tạo lại các Module và Lesson mới
                    if (updateCourseDto.CourseModules.Any())
                    {
                        foreach (var moduleData in updateCourseDto.CourseModules)
                        {
                            // Tạo Module mới
                            var module = new Module
                            {
                                Title = moduleData.Title,
                                Description = moduleData.Description,
                                Duration = !string.IsNullOrEmpty(moduleData.Duration) && double.TryParse(moduleData.Duration, out var duration) ? duration : null,
                                CourseId = course.Id,
                                CreatedAt = DateTime.Now,
                                Lessons = new List<long>()
                            };

                            _context.Modules.Add(module);
                            await _context.SaveChangesAsync();

                            // Tạo các Lesson cho Module này
                            var lessonIds = new List<long>();
                            if (moduleData.Lessons != null && moduleData.Lessons.Any())
                            {
                                foreach (var lessonData in moduleData.Lessons)
                                {
                                    var lesson = new Lesson
                                    {
                                        Name = lessonData.Name,
                                        VideoUrl = lessonData.VideoUrl,
                                        CourseId = course.Id,
                                        CoursePart = moduleData.Title,
                                        CreatedAt = DateTime.Now
                                    };

                                    _context.Lessons.Add(lesson);
                                    await _context.SaveChangesAsync();

                                    lessonIds.Add(lesson.Id);
                                }
                            }

                            // Cập nhật danh sách lesson IDs cho module
                            module.Lessons = lessonIds;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<CourseDto>(course);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteCourseAsync(long id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var course = await _context.Courses
                    .FirstOrDefaultAsync(c => c.Id == id && !c.Destroy);

                if (course == null)
                {
                    return false;
                }

                // Xóa các Module và Lesson liên quan (soft delete)
                var relatedModules = await _context.Modules
                    .Where(m => m.CourseId == id && !m.Destroy)
                    .ToListAsync();

                var relatedLessons = await _context.Lessons
                    .Where(l => l.CourseId == id && !l.Destroy)
                    .ToListAsync();

                foreach (var module in relatedModules)
                {
                    module.Destroy = true;
                    module.UpdatedAt = DateTime.Now;
                }

                foreach (var lesson in relatedLessons)
                {
                    lesson.Destroy = true;
                    lesson.UpdatedAt = DateTime.Now;
                }

                // Xóa Course
                course.Destroy = true;
                course.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
