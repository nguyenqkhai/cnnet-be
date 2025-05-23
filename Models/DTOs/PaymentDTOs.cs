namespace be_net.Models.DTOs
{
    public class MomoPaymentRequestDto
    {
        public long OrderId { get; set; }
        public string ReturnUrl { get; set; } = null!;
    }

    public class ZaloPaymentRequestDto
    {
        public long OrderId { get; set; }
        public string ReturnUrl { get; set; } = null!;
    }

    public class PaymentResponseDto
    {
        public string PaymentUrl { get; set; } = null!;
        public string OrderId { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
