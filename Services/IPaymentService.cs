using LmsBackend.DTOs;

namespace LmsBackend.Services
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreateMoMoPaymentAsync(PaymentRequestDto request);
        Task<PaymentResponseDto> CreateZaloPayPaymentAsync(PaymentRequestDto request);
        Task<bool> VerifyMoMoCallbackAsync(Dictionary<string, string> callbackData);
        Task<bool> VerifyZaloPayCallbackAsync(Dictionary<string, string> callbackData);
        Task<bool> ProcessPaymentCallbackAsync(PaymentCallbackDto callback);
    }
}
