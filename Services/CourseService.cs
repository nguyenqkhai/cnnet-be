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
            var course = _mapper.Map<Course>(createCourseDto);
            course.CreatedAt = DateTime.Now;

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return _mapper.Map<CourseDto>(course);
        }

        public async Task<CourseDto> UpdateCourseAsync(long id, UpdateCourseDto updateCourseDto)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == id && !c.Destroy);

            if (course == null)
            {
                throw new NotFoundException("Course not found");
            }

            _mapper.Map(updateCourseDto, course);
            course.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return _mapper.Map<CourseDto>(course);
        }

        public async Task<bool> DeleteCourseAsync(long id)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == id && !c.Destroy);

            if (course == null)
            {
                return false;
            }

            course.Destroy = true;
            course.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
