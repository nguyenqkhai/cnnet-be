using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LmsBackend.DTOs;
using LmsBackend.Services;

namespace LmsBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<ContactDto>>> GetAllContacts()
        {
            try
            {
                var contacts = await _contactService.GetAllContactsAsync();
                return Ok(contacts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách liên hệ", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ContactDto>> CreateContact([FromBody] CreateContactDto createContactDto)
        {
            try
            {
                var contact = await _contactService.CreateContactAsync(createContactDto);
                return Ok(contact);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo liên hệ", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteContact(long id)
        {
            try
            {
                var result = await _contactService.DeleteContactAsync(id);
                if (result)
                {
                    return Ok(new { message = "Đã xóa liên hệ thành công" });
                }
                return NotFound(new { message = "Không tìm thấy liên hệ" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa liên hệ", details = ex.Message });
            }
        }
    }
}
