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
                // Try environment variables first, then fallback to appsettings.json
                var partnerCode = Environment.GetEnvironmentVariable("MoMo_PartnerCode") ?? _configuration["MoMo:PartnerCode"];
                var accessKey = Environment.GetEnvironmentVariable("MoMo_AccessKey") ?? _configuration["MoMo:AccessKey"];
                var secretKey = Environment.GetEnvironmentVariable("MoMo_SecretKey") ?? _configuration["MoMo:SecretKey"];
                var endpoint = Environment.GetEnvironmentVariable("MoMo_Endpoint") ?? _configuration["MoMo:Endpoint"];

                // Validate configuration
                if (string.IsNullOrEmpty(partnerCode) || string.IsNullOrEmpty(accessKey) ||
                    string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(endpoint))
                {
                    Console.WriteLine($"❌ MoMo Configuration missing - PartnerCode: {partnerCode}, AccessKey: {accessKey}, SecretKey: {secretKey}, Endpoint: {endpoint}");
                    return new PaymentResponseDto
                    {
                        Success = false,
                        Message = "MoMo configuration is missing or incomplete",
                        PaymentMethod = "momo",
                        OrderId = request.OrderId
                    };
                }

                Console.WriteLine($"🔍 MoMo Config - PartnerCode: {partnerCode}, AccessKey: {accessKey}, Endpoint: {endpoint}");

                var requestId = Guid.NewGuid().ToString();
                var orderId = $"LMS_{request.OrderId}_{DateTime.Now:yyyyMMddHHmmss}";
                var orderInfo = request.Description;
                var amount = request.Amount;
                var extraData = "";

                var requestType = "payWithMethod"; // Allow multiple payment methods including cards

                // Create signature
                var rawSignature = $"accessKey={accessKey}&amount={amount}&extraData={extraData}&ipnUrl={request.NotifyUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={request.ReturnUrl}&requestId={requestId}&requestType={requestType}";
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
                    RequestType = requestType,
                    ExtraData = extraData,
                    Signature = signature
                };

                // Use JsonSerializer with proper options
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                var jsonRequest = JsonSerializer.Serialize(momoRequest, options);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                Console.WriteLine($"🔍 MoMo Request: {jsonRequest}");

                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"🔍 MoMo Response: {responseContent}");
                Console.WriteLine($"🔍 Response Status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    return new PaymentResponseDto
                    {
                        Success = false,
                        Message = $"MoMo API error: {response.StatusCode} - {responseContent}",
                        PaymentMethod = "momo",
                        OrderId = request.OrderId
                    };
                }

                var momoResponse = JsonSerializer.Deserialize<MoMoPaymentResponseDto>(responseContent, options);

                if (momoResponse == null)
                {
                    return new PaymentResponseDto
                    {
                        Success = false,
                        Message = "Failed to parse MoMo response",
                        PaymentMethod = "momo",
                        OrderId = request.OrderId
                    };
                }

                Console.WriteLine($"🔍 Parsed MoMo Response - ResultCode: {momoResponse.ResultCode}, PayUrl: {momoResponse.PayUrl}");

                return new PaymentResponseDto
                {
                    Success = momoResponse.ResultCode == 0, // Changed from "0" to 0
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
                // Try environment variables first, then fallback to appsettings.json
                var appIdStr = Environment.GetEnvironmentVariable("ZaloPay_AppId") ?? _configuration["ZaloPay:AppId"];
                var key1 = Environment.GetEnvironmentVariable("ZaloPay_Key1") ?? _configuration["ZaloPay:Key1"];
                var key2 = Environment.GetEnvironmentVariable("ZaloPay_Key2") ?? _configuration["ZaloPay:Key2"];
                var endpoint = Environment.GetEnvironmentVariable("ZaloPay_Endpoint") ?? _configuration["ZaloPay:Endpoint"];

                // Validate configuration
                if (string.IsNullOrEmpty(appIdStr) || string.IsNullOrEmpty(key1) ||
                    string.IsNullOrEmpty(key2) || string.IsNullOrEmpty(endpoint))
                {
                    Console.WriteLine($"❌ ZaloPay Configuration missing - AppId: {appIdStr}, Key1: {key1}, Key2: {key2}, Endpoint: {endpoint}");
                    return new PaymentResponseDto
                    {
                        Success = false,
                        Message = "ZaloPay configuration is missing or incomplete",
                        PaymentMethod = "zalopay",
                        OrderId = request.OrderId
                    };
                }

                if (!int.TryParse(appIdStr, out int appId))
                {
                    Console.WriteLine($"❌ ZaloPay AppId is not a valid integer: {appIdStr}");
                    return new PaymentResponseDto
                    {
                        Success = false,
                        Message = "ZaloPay AppId configuration is invalid",
                        PaymentMethod = "zalopay",
                        OrderId = request.OrderId
                    };
                }

                Console.WriteLine($"🔍 ZaloPay Config - AppId: {appId}, Key1: {key1}, Endpoint: {endpoint}");

                var appTransId = $"{DateTime.Now:yyMMdd}_{request.OrderId}_{DateTime.Now.Ticks}";
                var appTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var embedData = "{}"; // Empty JSON object
                var item = "[]"; // Empty JSON array

                // ZaloPay MAC format: app_id|app_trans_id|app_user|amount|app_time|embed_data|item
                var data = $"{appId}|{appTransId}|user123|{request.Amount}|{appTime}|{embedData}|{item}";
                var mac = CreateZaloPaySignature(data, key1);

                Console.WriteLine($"🔍 ZaloPay Data String: {data}");
                Console.WriteLine($"🔍 ZaloPay MAC: {mac}");

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

                Console.WriteLine($"🔍 ZaloPay Form Data: {string.Join(", ", formData.Select(x => $"{x.Key}={x.Value}"))}");

                var content = new FormUrlEncodedContent(formData);
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"🔍 ZaloPay Response: {responseContent}");
                Console.WriteLine($"🔍 ZaloPay Response Status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    return new PaymentResponseDto
                    {
                        Success = false,
                        Message = $"ZaloPay API error: {response.StatusCode} - {responseContent}",
                        PaymentMethod = "zalopay",
                        OrderId = request.OrderId
                    };
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };

                ZaloPayResponseDto? zaloResponse = null;
                try
                {
                    zaloResponse = JsonSerializer.Deserialize<ZaloPayResponseDto>(responseContent, options);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"❌ Failed to parse ZaloPay response: {ex.Message}");
                    Console.WriteLine($"❌ Response content: {responseContent}");

                    // Try parsing with camelCase as fallback
                    try
                    {
                        var fallbackOptions = new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            PropertyNameCaseInsensitive = true
                        };
                        zaloResponse = JsonSerializer.Deserialize<ZaloPayResponseDto>(responseContent, fallbackOptions);
                        Console.WriteLine("✅ Successfully parsed with camelCase fallback");
                    }
                    catch (JsonException fallbackEx)
                    {
                        Console.WriteLine($"❌ Fallback parsing also failed: {fallbackEx.Message}");
                        return new PaymentResponseDto
                        {
                            Success = false,
                            Message = $"Failed to parse ZaloPay response: {ex.Message}",
                            PaymentMethod = "zalopay",
                            OrderId = request.OrderId
                        };
                    }
                }

                if (zaloResponse == null)
                {
                    return new PaymentResponseDto
                    {
                        Success = false,
                        Message = "Failed to parse ZaloPay response",
                        PaymentMethod = "zalopay",
                        OrderId = request.OrderId
                    };
                }

                Console.WriteLine($"🔍 Parsed ZaloPay Response - ReturnCode: {zaloResponse.ReturnCode}, OrderUrl: {zaloResponse.OrderUrl}");

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
                Console.WriteLine($"❌ ZaloPay Payment Error: {ex.Message}");
                Console.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
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
                var secretKey = Environment.GetEnvironmentVariable("MoMo_SecretKey") ?? _configuration["MoMo:SecretKey"];
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
                var key2 = Environment.GetEnvironmentVariable("ZaloPay_Key2") ?? _configuration["ZaloPay:Key2"];
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
                Console.WriteLine($"🔍 Processing callback - OrderId: {callback.OrderId}, PaymentMethod: {callback.PaymentMethod}");

                // Extract order ID from callback based on payment method
                long orderId;
                if (callback.PaymentMethod == "momo")
                {
                    // MoMo format: LMS_{orderId}_{timestamp}
                    var orderIdStr = callback.OrderId.Replace("LMS_", "").Split('_')[0];
                    if (!long.TryParse(orderIdStr, out orderId))
                    {
                        Console.WriteLine($"❌ Failed to parse MoMo order ID: {callback.OrderId}");
                        return false;
                    }
                }
                else if (callback.PaymentMethod == "zalopay")
                {
                    // ZaloPay format: {yyMMdd}_{orderId}_{timestamp}
                    var parts = callback.OrderId.Split('_');
                    if (parts.Length < 2 || !long.TryParse(parts[1], out orderId))
                    {
                        Console.WriteLine($"❌ Failed to parse ZaloPay order ID: {callback.OrderId}");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine($"❌ Unknown payment method: {callback.PaymentMethod}");
                    return false;
                }

                Console.WriteLine($"🔍 Extracted order ID: {orderId}");

                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.Id == orderId && !o.Destroy);

                if (order == null)
                {
                    Console.WriteLine($"❌ Order not found: {orderId}");
                    return false;
                }

                Console.WriteLine($"🔍 Found order: {order.Id}, Current status: {order.Status}");

                // Update order status based on payment result
                if (callback.Status == "success" || callback.Status == "0" || callback.Status == "1")
                {
                    order.Status = "COMPLETED";
                    Console.WriteLine($"✅ Order {orderId} marked as COMPLETED");
                }
                else
                {
                    order.Status = "CANCELED";
                    Console.WriteLine($"❌ Order {orderId} marked as CANCELED");
                }

                order.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ProcessPaymentCallbackAsync Error: {ex.Message}");
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
