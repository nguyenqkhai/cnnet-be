using AutoMapper;
using Microsoft.EntityFrameworkCore;
using LmsBackend.Data;
using LmsBackend.DTOs;
using LmsBackend.Models;

namespace LmsBackend.Services
{
    public class ContactService : IContactService
    {
        private readonly LmsDbContext _context;
        private readonly IMapper _mapper;

        public ContactService(LmsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ContactDto>> GetAllContactsAsync()
        {
            var contacts = await _context.Contacts
                .Where(c => !c.Destroy)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return contacts.Select(c => new ContactDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                Message = c.Message,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).ToList();
        }

        public async Task<ContactDto> CreateContactAsync(CreateContactDto createContactDto)
        {
            var contact = new Contact
            {
                Name = createContactDto.Name,
                Email = createContactDto.Email,
                Phone = createContactDto.Phone,
                Message = createContactDto.Message,
                CreatedAt = DateTime.Now
            };

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return new ContactDto
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                Phone = contact.Phone,
                Message = contact.Message,
                CreatedAt = contact.CreatedAt,
                UpdatedAt = contact.UpdatedAt
            };
        }

        public async Task<bool> DeleteContactAsync(long id)
        {
            var contact = await _context.Contacts
                .FirstOrDefaultAsync(c => c.Id == id && !c.Destroy);

            if (contact == null)
            {
                return false;
            }

            contact.Destroy = true;
            contact.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
