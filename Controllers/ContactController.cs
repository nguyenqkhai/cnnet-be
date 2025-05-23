using be_net.Models;
using be_net.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace be_net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly CourseDBContext _context;
        private readonly ILogger<ContactController> _logger;

        public ContactController(CourseDBContext context, ILogger<ContactController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<ContactDto>>> GetContacts()
        {
            try
            {
                var contacts = await _context.Contacts
                    .Where(c => !c.Destroy)
                    .Select(c => new ContactDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Email = c.Email,
                        Phone = c.Phone,
                        Subject = c.Subject,
                        Message = c.Message,
                        CreatedAt = c.CreatedAt,
                        Destroy = c.Destroy
                    })
                    .ToListAsync();

                return Ok(contacts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contacts");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ContactDto>> CreateContact(ContactCreateDto contactCreateDto)
        {
            try
            {
                if (string.IsNullOrEmpty(contactCreateDto.Email) || string.IsNullOrEmpty(contactCreateDto.Name) || 
                    string.IsNullOrEmpty(contactCreateDto.Subject) || string.IsNullOrEmpty(contactCreateDto.Message))
                {
                    return BadRequest("Name, email, subject, and message are required");
                }

                var contact = new Contact
                {
                    Name = contactCreateDto.Name,
                    Email = contactCreateDto.Email,
                    Phone = contactCreateDto.Phone,
                    Subject = contactCreateDto.Subject,
                    Message = contactCreateDto.Message,
                    CreatedAt = DateTime.Now
                };

                _context.Contacts.Add(contact);
                await _context.SaveChangesAsync();

                var contactDto = new ContactDto
                {
                    Id = contact.Id,
                    Name = contact.Name,
                    Email = contact.Email,
                    Phone = contact.Phone,
                    Subject = contact.Subject,
                    Message = contact.Message,
                    CreatedAt = contact.CreatedAt,
                    Destroy = contact.Destroy
                };

                return CreatedAtAction(nameof(GetContacts), null, contactDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contact");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteContact(long id)
        {
            try
            {
                var contact = await _context.Contacts.FindAsync(id);

                if (contact == null)
                    return NotFound();

                // Soft delete
                contact.Destroy = true;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Contact deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting contact");
                return StatusCode(500, "An error occurred. Please try again later.");
            }
        }
    }
}
