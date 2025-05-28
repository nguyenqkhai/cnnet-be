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
                    // Update order payment method
                    await _orderService.UpdateOrderAsync(request.OrderId, new UpdateOrderDto 
                    { 
                        PaymentMethod = request.PaymentMethod 
                    });
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
                
                // Read form data
                foreach (var key in Request.Form.Keys)
                {
                    callbackData[key] = Request.Form[key];
                }

                // Verify signature
                var isValid = await _paymentService.VerifyMoMoCallbackAsync(callbackData);
                if (!isValid)
                {
                    return BadRequest("Invalid signature");
                }

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
                return StatusCode(500, new { message = "Callback processing failed", details = ex.Message });
            }
        }

        [HttpPost("zalopay/callback")]
        public async Task<IActionResult> ZaloPayCallback()
        {
            try
            {
                var callbackData = new Dictionary<string, string>();
                
                // Read form data
                foreach (var key in Request.Form.Keys)
                {
                    callbackData[key] = Request.Form[key];
                }

                // Verify signature
                var isValid = await _paymentService.VerifyZaloPayCallbackAsync(callbackData);
                if (!isValid)
                {
                    return BadRequest("Invalid signature");
                }

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
                return StatusCode(500, new { return_code = 0, return_message = "Callback processing failed" });
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
