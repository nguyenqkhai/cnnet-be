using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ElearningBackend.Data;
using ElearningBackend.DTOs;
using ElearningBackend.Models;

namespace ElearningBackend.Services
{
    public class BlogService : IBlogService
    {
        private readonly LmsDbContext _context;
        private readonly IMapper _mapper;

        public BlogService(LmsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<BlogDto>> GetAllBlogsAsync()
        {
            var blogs = await _context.Blogs
                .Where(b => !b.Destroy)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<BlogDto>>(blogs);
        }

        public async Task<BlogDto> GetBlogByIdAsync(long id)
        {
            var blog = await _context.Blogs
                .FirstOrDefaultAsync(b => b.Id == id && !b.Destroy);

            if (blog == null)
            {
                throw new NotFoundException("Blog not found");
            }

            return _mapper.Map<BlogDto>(blog);
        }

        public async Task<BlogDto> CreateBlogAsync(CreateBlogDto createBlogDto)
        {
            var blog = _mapper.Map<Blog>(createBlogDto);
            blog.CreatedAt = DateTime.Now;

            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            return _mapper.Map<BlogDto>(blog);
        }

        public async Task<BlogDto> UpdateBlogAsync(long id, UpdateBlogDto updateBlogDto)
        {
            var blog = await _context.Blogs
                .FirstOrDefaultAsync(b => b.Id == id && !b.Destroy);

            if (blog == null)
            {
                throw new NotFoundException("Blog not found");
            }

            _mapper.Map(updateBlogDto, blog);
            blog.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return _mapper.Map<BlogDto>(blog);
        }

        public async Task<bool> DeleteBlogAsync(long id)
        {
            var blog = await _context.Blogs
                .FirstOrDefaultAsync(b => b.Id == id && !b.Destroy);

            if (blog == null)
            {
                return false;
            }

            blog.Destroy = true;
            blog.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
