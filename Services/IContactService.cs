using ElearningBackend.DTOs;

namespace ElearningBackend.Services
{
    public interface IContactService
    {
        Task<List<ContactDto>> GetAllContactsAsync();
        Task<ContactDto> CreateContactAsync(CreateContactDto createContactDto);
        Task<bool> DeleteContactAsync(long id);
    }
}
