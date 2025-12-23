## 🎓 CNNET / LMS Backend (ASP.NET Core 8)

Backend cho hệ thống quản lý khóa học trực tuyến (Learning Management System – LMS), cung cấp API phục vụ các chức năng: quản lý người dùng & khóa học, giỏ hàng/đơn hàng, thanh toán online, blog, đánh giá, wishlist, voucher và upload media.

Dự án được xây dựng theo mô hình Web API, tách lớp rõ ràng, hướng tới môi trường production.
Swagger được bật mặc định ở mọi môi trường để dễ kiểm thử API.

## 🚀 Các chức năng chính

- Authentication & Authorization
- Đăng ký / đăng nhập
- JWT Bearer Authentication
- Phân quyền truy cập API
- Quản lý khóa học
- Course / Module / Lesson CRUD
- Theo dõi tiến độ học tập (progress)
- Giỏ hàng & đơn hàng
- Thêm/xóa khóa học vào giỏ
- Tạo đơn hàng và xử lý trạng thái
- Thanh toán
- Tích hợp MoMo & ZaloPay
- Tạo link thanh toán
- Xử lý callback, cập nhật trạng thái đơn hàng
- Voucher
- Tạo & áp mã giảm giá
- Ràng buộc số lần sử dụng
- Unique code (index DB)
- Wishlist & đánh giá
- Lưu khóa học yêu thích
- Review & rating khóa học
- Blog & nội dung
- Blog, comment, contact
- Media
- Upload hình ảnh/video thông qua Cloudinary

## 🧠 Backend Highlights

- JWT Authentication & Role-based Authorization
- Xử lý transaction cho luồng Order – Payment
- Xác thực callback thanh toán (MoMo, ZaloPay)
- Thiết kế API RESTful theo chuẩn
- Tách lớp Controller – Service – DTO rõ ràng
- Mapping dữ liệu bằng AutoMapper
- Upload & quản lý media trên Cloudinary

## 🛠️ Công nghệ sử dụng

- .NET 8, ASP.NET Core Web API
- Entity Framework Core (SQL Server)
- JWT Authentication & Authorization
- AutoMapper, Newtonsoft.Json
- Cloudinary (upload media)
- MoMo & ZaloPay (sandbox)

## 📂 Cấu trúc thư mục

```
├── Controllers/ // API Controllers (Auth, Course, Payment, Order, Voucher…)
├── Services/ // Business logic + Interfaces (Dependency Injection)
├── Models/ // Entity (EF Core)
├── DTOs/ // Data Transfer Objects
├── Data/
│ └── LmsDbContext.cs // DbContext & quan hệ bảng
├── Mappings/ // AutoMapper profiles
├── Properties/ // Launch settings
├── deploy.md // Hướng dẫn deploy Azure
```

## ⚙️ Yêu cầu môi trường

- .NET 8 SDK
- SQL Server (local hoặc cloud)
- Tài khoản Cloudinary
- Tài khoản sandbox MoMo & ZaloPay (nếu test thanh toán)

## ▶️ Chạy project local

- Cài đặt .NET 8 SDK và SQL Server
- Chuẩn bị database phù hợp với các model EF Core
- Restore package: `dotnet restore`
- Chạy ứng dụng: `dotnet run`
- Truy cập Swagger: `http://localhost:{port}/swagger`
