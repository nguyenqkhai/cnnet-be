using be_net.Models;
using be_net.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using System.Net.Http;
using System.Text.Json;

namespace be_net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly CourseDBContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentController> _logger;
        private readonly HttpClient _httpClient;

        public PaymentController(CourseDBContext context, IConfiguration configuration, ILogger<PaymentController> logger, HttpClient httpClient)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
        }

        [HttpPost("momo")]
        public async Task<ActionResult<PaymentResponseDto>> MomoPayment(MomoPaymentRequestDto momoPaymentRequestDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var order = await _context.Orders.FindAsync(momoPaymentRequestDto.OrderId);
                if (order == null || order.Destroy)
                    return NotFound("Order not found");

                // Ensure the user is only paying for their own order
                if (order.UserId != long.Parse(userId))
                    return Forbid();

                // Check if order is already paid
                if (order.Status == "completed")
                    return BadRequest("Order is already paid");

                // Momo payment integration
                // Note: This is a simplified example. In a real-world scenario, you would need to integrate with Momo's API.
                var momoEndpoint = _configuration["Payment:Momo:Endpoint"];
                var partnerCode = _configuration["Payment:Momo:PartnerCode"];
                var accessKey = _configuration["Payment:Momo:AccessKey"];
                var secretKey = _configuration["Payment:Momo:SecretKey"];
                var ipnUrl = _configuration["Payment:Momo:IpnUrl"];
                var redirectUrl = momoPaymentRequestDto.ReturnUrl;
                var orderId = order.Id.ToString();
                var amount = order.TotalPrice.ToString();
                var orderInfo = $"Payment for course: {order.CourseName}";
                var requestId = Guid.NewGuid().ToString();
                var extraData = "";

                // Check if secretKey is null
                if (string.IsNullOrEmpty(secretKey))
                {
                    _logger.LogError("Momo secret key is not configured");
                    return StatusCode(500, "Payment configuration error. Please contact support.");
                }

                // Create signature
                var rawHash = $"partnerCode={partnerCode}&accessKey={accessKey}&requestId={requestId}&amount={amount}&orderId={orderId}&orderInfo={orderInfo}&returnUrl={redirectUrl}&notifyUrl={ipnUrl}&extraData={extraData}";
                var signature = ComputeHmacSha256(rawHash, secretKey);

                // Create request body
                var requestBody = new
                {
                    partnerCode,
                    accessKey,
                    requestId,
                    amount,
                    orderId,
                    orderInfo,
                    returnUrl = redirectUrl,
                    notifyUrl = ipnUrl,
                    extraData,
                    requestType = "captureMoMoWallet",
                    signature
                };

                // Send request to Momo
                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(momoEndpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var responseData = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    var payUrl = responseData.GetProperty("payUrl").GetString() ?? string.Empty;

                    return Ok(new PaymentResponseDto
                    {
                        PaymentUrl = payUrl,
                        OrderId = orderId,
                        Message = "Momo payment URL generated successfully"
                    });
                }
                else
                {
                    _logger.LogError($"Momo payment error: {responseContent}");
                    return StatusCode(500, "An error occurred while processing the payment. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Momo payment");
                return StatusCode(500, "An error occurred while processing the payment. Please try again later.");
            }
        }

        [HttpPost("zalopay")]
        public async Task<ActionResult<PaymentResponseDto>> ZaloPayment(ZaloPaymentRequestDto zaloPaymentRequestDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var order = await _context.Orders.FindAsync(zaloPaymentRequestDto.OrderId);
                if (order == null || order.Destroy)
                    return NotFound("Order not found");

                // Ensure the user is only paying for their own order
                if (order.UserId != long.Parse(userId))
                    return Forbid();

                // Check if order is already paid
                if (order.Status == "completed")
                    return BadRequest("Order is already paid");

                // ZaloPay payment integration
                // Note: This is a simplified example. In a real-world scenario, you would need to integrate with ZaloPay's API.
                var zaloEndpoint = _configuration["Payment:ZaloPay:Endpoint"];
                var appId = _configuration["Payment:ZaloPay:AppId"];
                var key1 = _configuration["Payment:ZaloPay:Key1"];
                var key2 = _configuration["Payment:ZaloPay:Key2"];
                var appUser = userId;
                var appTime = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
                var amount = order.TotalPrice.ToString();
                var embedData = JsonSerializer.Serialize(new { redirecturl = zaloPaymentRequestDto.ReturnUrl });
                var items = JsonSerializer.Serialize(new[] { new { name = order.CourseName, amount = amount } });
                var description = $"Payment for course: {order.CourseName}";
                var bankCode = "";

                // Check if key1 is null
                if (string.IsNullOrEmpty(key1))
                {
                    _logger.LogError("ZaloPay key1 is not configured");
                    return StatusCode(500, "Payment configuration error. Please contact support.");
                }

                // Create mac
                var data = $"{appId}|{order.Id}|{amount}|{appTime}|{embedData}|{items}";
                var mac = ComputeHmacSha256(data, key1);

                // Create request body
                var requestBody = new
                {
                    app_id = appId,
                    app_user = appUser,
                    app_time = appTime,
                    amount,
                    app_trans_id = order.Id.ToString(),
                    embed_data = embedData,
                    item = items,
                    description,
                    bank_code = bankCode,
                    mac
                };

                // Send request to ZaloPay
                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(zaloEndpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var responseData = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    var returnCode = responseData.GetProperty("return_code").GetInt32();

                    if (returnCode == 1)
                    {
                        var orderUrl = responseData.GetProperty("order_url").GetString() ?? string.Empty;

                        return Ok(new PaymentResponseDto
                        {
                            PaymentUrl = orderUrl,
                            OrderId = order.Id.ToString(),
                            Message = "ZaloPay payment URL generated successfully"
                        });
                    }
                    else
                    {
                        var returnMessage = responseData.GetProperty("return_message").GetString();
                        _logger.LogError($"ZaloPay payment error: {returnMessage}");
                        return BadRequest(returnMessage);
                    }
                }
                else
                {
                    _logger.LogError($"ZaloPay payment error: {responseContent}");
                    return StatusCode(500, "An error occurred while processing the payment. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing ZaloPay payment");
                return StatusCode(500, "An error occurred while processing the payment. Please try again later.");
            }
        }

        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(messageBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
