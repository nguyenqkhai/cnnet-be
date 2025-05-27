using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LmsBackend.DTOs;

namespace LmsBackend.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var cloudinarySettings = configuration.GetSection("Cloudinary");
            var cloudName = cloudinarySettings["CloudName"];
            var apiKey = cloudinarySettings["ApiKey"];
            var apiSecret = cloudinarySettings["ApiSecret"];
            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<UploadResponseDto> UploadImageAsync(IFormFile file, string folder = "images")
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required");

            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                throw new ArgumentException("Only image files are allowed");

            // Xác thực kích thước file (tối đa 10MB)
            if (file.Length > 10 * 1024 * 1024)
                throw new ArgumentException("File size must be less than 10MB");

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                Transformation = new Transformation().Quality("auto").FetchFormat("auto")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
                throw new Exception($"Upload failed: {uploadResult.Error.Message}");

            return new UploadResponseDto
            {
                Url = uploadResult.SecureUrl.ToString(),
                FileName = uploadResult.PublicId,
                Size = file.Length
            };
        }

        public async Task<UploadResponseDto> UploadVideoAsync(IFormFile file, string folder = "videos")
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required");

            var allowedTypes = new[] { "video/mp4", "video/avi", "video/mov", "video/wmv", "video/flv", "video/webm" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                throw new ArgumentException("Only video files are allowed");

            // Xác thực kích thước file (tối đa 100MB)
            if (file.Length > 100 * 1024 * 1024)
                throw new ArgumentException("File size must be less than 100MB");

            using var stream = file.OpenReadStream();
            var uploadParams = new VideoUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
                throw new Exception($"Upload failed: {uploadResult.Error.Message}");

            return new UploadResponseDto
            {
                Url = uploadResult.SecureUrl.ToString(),
                FileName = uploadResult.PublicId,
                Size = file.Length
            };
        }

        public async Task<bool> DeleteFileAsync(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                return false;

            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result.Result == "ok";
        }

        public async Task<UploadResponseDto> UploadUserImageAsync(IFormFile file)
        {
            return await UploadImageAsync(file, "users");
        }

        public async Task<UploadResponseDto> UploadBlogImageAsync(IFormFile file)
        {
            return await UploadImageAsync(file, "blogs");
        }

        public async Task<UploadResponseDto> UploadCourseImageAsync(IFormFile file)
        {
            return await UploadImageAsync(file, "courses");
        }

        public async Task<UploadResponseDto> UploadCourseVideoAsync(IFormFile file)
        {
            return await UploadVideoAsync(file, "course-videos");
        }
    }
}
