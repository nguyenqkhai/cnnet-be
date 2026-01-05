using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ElearningBackend.Data;
using ElearningBackend.DTOs;
using ElearningBackend.Models;

namespace ElearningBackend.Services
{
    public class UserService : IUserService
    {
        private readonly LmsDbContext _context;
        private readonly IMapper _mapper;

        public UserService(LmsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Where(u => !u.Destroy)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                Username = u.Username,
                Avatar = u.Avatar,
                Role = u.Role,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            }).ToList();
        }

        public async Task<UserDto> UpdateUserAsync(long id, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && !u.Destroy);

            if (user == null)
            {
                throw new NotFoundException("User not found");
            }
            if (!string.IsNullOrEmpty(updateUserDto.Email) && updateUserDto.Email != user.Email)
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == updateUserDto.Email.ToLower() &&
                                             u.Id != id && !u.Destroy);

                if (existingUser != null)
                {
                    throw new InvalidOperationException("Email already exists");
                }
            }
            if (!string.IsNullOrEmpty(updateUserDto.Email))
                user.Email = updateUserDto.Email;
            if (!string.IsNullOrEmpty(updateUserDto.Username))
                user.Username = updateUserDto.Username;
            if (updateUserDto.Avatar != null)
                user.Avatar = updateUserDto.Avatar;

            user.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                Avatar = user.Avatar,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}
