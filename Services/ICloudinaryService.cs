using LmsBackend.DTOs;

namespace LmsBackend.Services
{
    public interface ICloudinaryService
    {
        Task<UploadResponseDto> UploadImageAsync(IFormFile file, string folder = "images");
        Task<UploadResponseDto> UploadVideoAsync(IFormFile file, string folder = "videos");
        Task<bool> DeleteFileAsync(string publicId);
        Task<UploadResponseDto> UploadUserImageAsync(IFormFile file);
        Task<UploadResponseDto> UploadBlogImageAsync(IFormFile file);
        Task<UploadResponseDto> UploadCourseImageAsync(IFormFile file);
        Task<UploadResponseDto> UploadCourseVideoAsync(IFormFile file);
    }
}
