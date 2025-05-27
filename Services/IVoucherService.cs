using LmsBackend.DTOs;

namespace LmsBackend.Services
{
    public interface IVoucherService
    {
        Task<List<VoucherDto>> GetAllVouchersAsync();
        Task<VoucherDto> CreateVoucherAsync(CreateVoucherDto createVoucherDto);
        Task<VoucherDto?> FindVoucherByNameAsync(string name);
        Task<VoucherDto> UpdateVoucherAsync(long id, UpdateVoucherDto updateVoucherDto);
        Task<bool> DeleteVoucherAsync(long id);
        Task<VoucherDto?> ValidateVoucherAsync(string code, long courseId, int orderValue);
        Task<VoucherDto> UseVoucherAsync(long voucherId);
    }
}
