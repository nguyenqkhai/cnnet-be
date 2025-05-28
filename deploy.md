# Azure Deployment Guide

## Các thay đổi đã thực hiện để khắc phục lỗi 404:

### 1. Cập nhật Program.cs
- ✅ Bật Swagger trong tất cả environments (không chỉ Development)
- ✅ Thêm default route "/" redirect đến "/swagger"
- ✅ Thêm health check endpoint "/health"
- ✅ Cấu hình Swagger UI với route prefix rõ ràng

### 2. Thêm web.config
- ✅ Cấu hình ASP.NET Core Module cho IIS/Azure App Service
- ✅ Đảm bảo hosting model inprocess

### 3. Thêm appsettings.Production.json
- ✅ Cấu hình logging cho production
- ✅ Tối ưu performance

## Kiểm tra trước khi deploy:

### 1. Kiểm tra Azure App Service Configuration
Đảm bảo các environment variables sau được thiết lập trong Azure Portal:

```
ConnectionStrings__DefaultConnection = Server=nguyenqkhai.database.windows.net;Database=COURSE_DB_CNNET;User Id=nguyenqkhai;Password=Khai@123;TrustServerCertificate=true;

JwtSettings__SecretKey = Lms2025SecretKey!@#$%^&*()_+ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789
JwtSettings__Issuer = LmsBackend
JwtSettings__Audience = LmsBackend
JwtSettings__ExpiryInMinutes = 1440

Cloudinary__CloudName = dybxa6g1r
Cloudinary__ApiKey = 512378497311973
Cloudinary__ApiSecret = YrjJ3UoDBfIUeS4mJOgfYiy7K-0
```

### 2. Kiểm tra Azure App Service Settings
- ✅ .NET Version: 8.0
- ✅ Platform: 64-bit
- ✅ Always On: Enabled (nếu không phải Free tier)

### 3. Test endpoints sau khi deploy:
- `https://your-app.azurewebsites.net/` → Should redirect to Swagger
- `https://your-app.azurewebsites.net/swagger` → Swagger UI
- `https://your-app.azurewebsites.net/health` → Health check
- `https://your-app.azurewebsites.net/api/auth/register` → API endpoint

## Troubleshooting:

### Nếu vẫn gặp lỗi 404:
1. Kiểm tra Azure App Service Logs
2. Đảm bảo file web.config được deploy
3. Kiểm tra environment variables
4. Restart Azure App Service

### Nếu gặp lỗi 500:
1. Kiểm tra connection string database
2. Kiểm tra JWT settings
3. Kiểm tra Cloudinary settings
4. Xem Application Insights logs

## Commands để deploy:

### Via GitHub (Recommended):
1. Push code lên GitHub
2. Azure sẽ tự động deploy

### Via Azure CLI:
```bash
az webapp deployment source config-zip --resource-group your-rg --name your-app --src publish.zip
```

### Via Visual Studio:
1. Right-click project → Publish
2. Choose Azure App Service
3. Follow wizard
