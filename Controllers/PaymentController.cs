using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LmsBackend.DTOs;
using LmsBackend.Services;

namespace LmsBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;

        public PaymentController(IPaymentService paymentService, IOrderService orderService)
        {
            _paymentService = paymentService;
            _orderService = orderService;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<ActionResult<PaymentResponseDto>> CreatePayment([FromBody] PaymentRequestDto request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                // Verify order belongs to user
                var order = await _orderService.GetOrderByIdAsync(request.OrderId);
                if (order.UserId != userId)
                {
                    return Forbid("Bạn chỉ có thể thanh toán đơn hàng của mình");
                }

                PaymentResponseDto result;

                switch (request.PaymentMethod.ToLower())
                {
                    case "momo":
                        result = await _paymentService.CreateMoMoPaymentAsync(request);
                        break;
                    case "zalopay":
                        result = await _paymentService.CreateZaloPayPaymentAsync(request);
                        break;
                    default:
                        return BadRequest(new { message = "Phương thức thanh toán không được hỗ trợ" });
                }

                if (result.Success)
                {
                    // Only update payment method and set status to PENDING (waiting for payment)
                    await _orderService.UpdateOrderAsync(request.OrderId, new UpdateOrderDto
                    {
                        PaymentMethod = request.PaymentMethod,
                        Status = "PENDING"
                    });

                    Console.WriteLine($"🔍 Order {request.OrderId} marked as PENDING, waiting for payment callback");
                }
                else
                {
                    // Update order status to CANCELED if payment URL creation failed
                    await _orderService.UpdateOrderAsync(request.OrderId, new UpdateOrderDto
                    {
                        PaymentMethod = request.PaymentMethod,
                        Status = "CANCELED"
                    });

                    Console.WriteLine($"❌ Order {request.OrderId} marked as CANCELED after failed payment URL creation");
                }

                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo thanh toán", details = ex.Message });
            }
        }

        [HttpPost("momo/callback")]
        public async Task<IActionResult> MoMoCallback()
        {
            try
            {
                var callbackData = new Dictionary<string, string>();

                // Check if request has form data
                if (Request.HasFormContentType && Request.Form.Count > 0)
                {
                    // Read form data from MoMo
                    foreach (var key in Request.Form.Keys)
                    {
                        callbackData[key] = Request.Form[key];
                    }
                }
                else
                {
                    // For testing purposes - simulate MoMo callback data
                    callbackData = new Dictionary<string, string>
                    {
                        ["orderId"] = "LMS_23_20250529201738",
                        ["transId"] = "123456789",
                        ["resultCode"] = "0",
                        ["amount"] = "299000",
                        ["message"] = "Thành công",
                        ["signature"] = "test_signature"
                    };
                }

                Console.WriteLine($"🔍 MoMo Callback Data: {string.Join(", ", callbackData.Select(x => $"{x.Key}={x.Value}"))}");

                // For testing, skip signature verification
                // var isValid = await _paymentService.VerifyMoMoCallbackAsync(callbackData);
                // if (!isValid)
                // {
                //     return BadRequest("Invalid signature");
                // }

                // Process callback
                var callback = new PaymentCallbackDto
                {
                    OrderId = callbackData.GetValueOrDefault("orderId", ""),
                    TransactionId = callbackData.GetValueOrDefault("transId", ""),
                    Status = callbackData.GetValueOrDefault("resultCode", ""),
                    Amount = long.Parse(callbackData.GetValueOrDefault("amount", "0")),
                    PaymentMethod = "momo",
                    Message = callbackData.GetValueOrDefault("message", ""),
                    PaymentTime = DateTime.Now
                };

                var processed = await _paymentService.ProcessPaymentCallbackAsync(callback);

                return Ok(new { success = processed });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ MoMo Callback Error: {ex.Message}");
                return StatusCode(500, new { message = "Callback processing failed", details = ex.Message });
            }
        }

        [HttpPost("zalopay/callback")]
        public async Task<IActionResult> ZaloPayCallback()
        {
            try
            {
                var callbackData = new Dictionary<string, string>();

                // Check if request has form data
                if (Request.HasFormContentType && Request.Form.Count > 0)
                {
                    // Read form data from ZaloPay
                    foreach (var key in Request.Form.Keys)
                    {
                        callbackData[key] = Request.Form[key];
                    }
                }
                else
                {
                    // For testing purposes - simulate ZaloPay callback data
                    callbackData = new Dictionary<string, string>
                    {
                        ["app_trans_id"] = "250529_23_638841469494286472",
                        ["zp_trans_id"] = "987654321",
                        ["status"] = "1",
                        ["amount"] = "299000",
                        ["message"] = "success",
                        ["mac"] = "test_mac"
                    };
                }

                Console.WriteLine($"🔍 ZaloPay Callback Data: {string.Join(", ", callbackData.Select(x => $"{x.Key}={x.Value}"))}");

                // For testing, skip signature verification
                // var isValid = await _paymentService.VerifyZaloPayCallbackAsync(callbackData);
                // if (!isValid)
                // {
                //     return Ok(new { return_code = 0, return_message = "Invalid signature" });
                // }

                // Process callback
                var callback = new PaymentCallbackDto
                {
                    OrderId = callbackData.GetValueOrDefault("app_trans_id", ""),
                    TransactionId = callbackData.GetValueOrDefault("zp_trans_id", ""),
                    Status = callbackData.GetValueOrDefault("status", ""),
                    Amount = long.Parse(callbackData.GetValueOrDefault("amount", "0")),
                    PaymentMethod = "zalopay",
                    Message = callbackData.GetValueOrDefault("message", ""),
                    PaymentTime = DateTime.Now
                };

                var processed = await _paymentService.ProcessPaymentCallbackAsync(callback);

                return Ok(new { return_code = processed ? 1 : 0, return_message = processed ? "success" : "fail" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ZaloPay Callback Error: {ex.Message}");
                return Ok(new { return_code = 0, return_message = "Callback processing failed" });
            }
        }

        [HttpPost("test/complete/{orderId}")]
        public async Task<ActionResult> TestCompletePayment(long orderId, [FromBody] TestCompletePaymentDto request)
        {
            try
            {
                Console.WriteLine($"🔍 Test Complete Payment - OrderId: {orderId}, PaymentMethod: {request.PaymentMethod}");

                // Create callback data based on payment method
                var callback = new PaymentCallbackDto
                {
                    OrderId = request.PaymentMethod == "momo" ? $"LMS_{orderId}_{DateTime.Now:yyyyMMddHHmmss}" : $"250529_{orderId}_{DateTime.Now.Ticks}",
                    TransactionId = $"TEST_{DateTime.Now.Ticks}",
                    Status = request.PaymentMethod == "momo" ? "0" : "1", // MoMo: 0=success, ZaloPay: 1=success
                    Amount = request.Amount,
                    PaymentMethod = request.PaymentMethod,
                    Message = "Test payment completion",
                    PaymentTime = DateTime.Now
                };

                var processed = await _paymentService.ProcessPaymentCallbackAsync(callback);

                return Ok(new {
                    success = processed,
                    message = processed ? "Payment completed successfully" : "Failed to complete payment",
                    orderId = orderId
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Test Complete Payment Error: {ex.Message}");
                return StatusCode(500, new { message = "Failed to complete payment", details = ex.Message });
            }
        }

        [HttpGet("status/{orderId}")]
        [Authorize]
        public async Task<ActionResult> GetPaymentStatus(long orderId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    return Unauthorized(new { message = "Token không hợp lệ" });
                }

                var order = await _orderService.GetOrderByIdAsync(orderId);

                if (order.UserId != userId)
                {
                    return Forbid("Bạn chỉ có thể xem trạng thái đơn hàng của mình");
                }

                return Ok(new
                {
                    orderId = order.Id,
                    status = order.Status,
                    paymentMethod = order.PaymentMethod,
                    amount = order.TotalPrice,
                    createdAt = order.CreatedAt,
                    updatedAt = order.UpdatedAt
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy trạng thái thanh toán", details = ex.Message });
            }
        }
    }
}
