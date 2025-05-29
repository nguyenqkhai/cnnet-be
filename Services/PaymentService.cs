using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using LmsBackend.Data;
using LmsBackend.DTOs;

namespace LmsBackend.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly LmsDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public PaymentService(LmsDbContext context, IConfiguration configuration, HttpClient httpClient)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<PaymentResponseDto> CreateMoMoPaymentAsync(PaymentRequestDto request)
        {
            try
            {
                var momoConfig = _configuration.GetSection("MoMo");
                var partnerCode = momoConfig["PartnerCode"];
                var accessKey = momoConfig["AccessKey"];
                var secretKey = momoConfig["SecretKey"];
                var endpoint = momoConfig["Endpoint"];

                Console.WriteLine($"🔍 MoMo Config - PartnerCode: {partnerCode}, AccessKey: {accessKey}, Endpoint: {endpoint}");

                var requestId = Guid.NewGuid().ToString();
                var orderId = $"LMS_{request.OrderId}_{DateTime.Now:yyyyMMddHHmmss}";
                var orderInfo = request.Description;
                var amount = request.Amount;
                var extraData = "";

                // Create signature
                var rawSignature = $"accessKey={accessKey}&amount={amount}&extraData={extraData}&ipnUrl={request.NotifyUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={request.ReturnUrl}&requestId={requestId}&requestType=captureWallet";
                var signature = CreateMoMoSignature(rawSignature, secretKey);

                var momoRequest = new MoMoPaymentRequestDto
                {
                    PartnerCode = partnerCode,
                    RequestId = requestId,
                    Amount = amount,
                    OrderId = orderId,
                    OrderInfo = orderInfo,
                    RedirectUrl = request.ReturnUrl,
                    IpnUrl = request.NotifyUrl,
                    ExtraData = extraData,
                    Signature = signature
                };

                var jsonRequest = JsonSerializer.Serialize(momoRequest);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                Console.WriteLine($"🔍 MoMo Request: {jsonRequest}");

                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"🔍 MoMo Response: {responseContent}");

                var momoResponse = JsonSerializer.Deserialize<MoMoPaymentResponseDto>(responseContent);

                return new PaymentResponseDto
                {
                    Success = momoResponse.ResultCode == "0",
                    PaymentUrl = momoResponse.PayUrl,
                    TransactionId = momoResponse.RequestId,
                    Message = momoResponse.Message,
                    PaymentMethod = "momo",
                    OrderId = request.OrderId
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ MoMo Payment Error: {ex.Message}");
                Console.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
                return new PaymentResponseDto
                {
                    Success = false,
                    Message = $"MoMo payment creation failed: {ex.Message}",
                    PaymentMethod = "momo",
                    OrderId = request.OrderId
                };
            }
        }

        public async Task<PaymentResponseDto> CreateZaloPayPaymentAsync(PaymentRequestDto request)
        {
            try
            {
                var zaloConfig = _configuration.GetSection("ZaloPay");
                var appId = int.Parse(zaloConfig["AppId"]);
                var key1 = zaloConfig["Key1"];
                var key2 = zaloConfig["Key2"];
                var endpoint = zaloConfig["Endpoint"];

                var appTransId = $"{DateTime.Now:yyMMdd}_{request.OrderId}_{DateTime.Now.Ticks}";
                var appTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var embedData = JsonSerializer.Serialize(new { orderId = request.OrderId });
                var item = JsonSerializer.Serialize(new[] { new { name = request.Description, quantity = 1, price = request.Amount } });

                var data = $"{appId}|{appTransId}|{request.Amount}|{request.Description}|{appTime}|{embedData}|{item}";
                var mac = CreateZaloPaySignature(data, key1);

                var zaloRequest = new ZaloPayRequestDto
                {
                    AppId = appId,
                    AppUser = "user123",
                    AppTime = appTime,
                    Amount = request.Amount,
                    AppTransId = appTransId,
                    EmbedData = embedData,
                    Item = item,
                    Description = request.Description,
                    BankCode = "",
                    Mac = mac,
                    CallbackUrl = request.NotifyUrl
                };

                var formData = new List<KeyValuePair<string, string>>
                {
                    new("app_id", zaloRequest.AppId.ToString()),
                    new("app_user", zaloRequest.AppUser),
                    new("app_time", zaloRequest.AppTime.ToString()),
                    new("amount", zaloRequest.Amount.ToString()),
                    new("app_trans_id", zaloRequest.AppTransId),
                    new("embed_data", zaloRequest.EmbedData),
                    new("item", zaloRequest.Item),
                    new("description", zaloRequest.Description),
                    new("bank_code", zaloRequest.BankCode),
                    new("mac", zaloRequest.Mac),
                    new("callback_url", zaloRequest.CallbackUrl)
                };

                var content = new FormUrlEncodedContent(formData);
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                var zaloResponse = JsonSerializer.Deserialize<ZaloPayResponseDto>(responseContent);

                return new PaymentResponseDto
                {
                    Success = zaloResponse.ReturnCode == 1,
                    PaymentUrl = zaloResponse.OrderUrl,
                    TransactionId = zaloResponse.AppTransId,
                    Message = zaloResponse.ReturnMessage,
                    PaymentMethod = "zalopay",
                    OrderId = request.OrderId
                };
            }
            catch (Exception ex)
            {
                return new PaymentResponseDto
                {
                    Success = false,
                    Message = $"ZaloPay payment creation failed: {ex.Message}",
                    PaymentMethod = "zalopay",
                    OrderId = request.OrderId
                };
            }
        }

        public async Task<bool> VerifyMoMoCallbackAsync(Dictionary<string, string> callbackData)
        {
            try
            {
                var secretKey = _configuration.GetSection("MoMo")["SecretKey"];
                var signature = callbackData.GetValueOrDefault("signature", "");

                // Recreate signature for verification
                var rawSignature = $"accessKey={callbackData.GetValueOrDefault("accessKey")}&amount={callbackData.GetValueOrDefault("amount")}&extraData={callbackData.GetValueOrDefault("extraData")}&message={callbackData.GetValueOrDefault("message")}&orderId={callbackData.GetValueOrDefault("orderId")}&orderInfo={callbackData.GetValueOrDefault("orderInfo")}&orderType={callbackData.GetValueOrDefault("orderType")}&partnerCode={callbackData.GetValueOrDefault("partnerCode")}&payType={callbackData.GetValueOrDefault("payType")}&requestId={callbackData.GetValueOrDefault("requestId")}&responseTime={callbackData.GetValueOrDefault("responseTime")}&resultCode={callbackData.GetValueOrDefault("resultCode")}&transId={callbackData.GetValueOrDefault("transId")}";

                var expectedSignature = CreateMoMoSignature(rawSignature, secretKey);
                return signature == expectedSignature;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> VerifyZaloPayCallbackAsync(Dictionary<string, string> callbackData)
        {
            try
            {
                var key2 = _configuration.GetSection("ZaloPay")["Key2"];
                var mac = callbackData.GetValueOrDefault("mac", "");

                var data = $"{callbackData.GetValueOrDefault("data")}";
                var expectedMac = CreateZaloPaySignature(data, key2);

                return mac == expectedMac;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ProcessPaymentCallbackAsync(PaymentCallbackDto callback)
        {
            try
            {
                // Extract order ID from callback
                var orderIdStr = callback.OrderId.Replace("LMS_", "").Split('_')[0];
                if (!long.TryParse(orderIdStr, out long orderId))
                {
                    return false;
                }

                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.Id == orderId && !o.Destroy);

                if (order == null)
                {
                    return false;
                }

                // Update order status based on payment result
                if (callback.Status == "success" || callback.Status == "0")
                {
                    order.Status = "completed";
                }
                else
                {
                    order.Status = "failed";
                }

                order.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private string CreateMoMoSignature(string rawSignature, string secretKey)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawSignature));
            return Convert.ToHexString(hash).ToLower();
        }

        private string CreateZaloPaySignature(string data, string key)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToHexString(hash).ToLower();
        }
    }
}
