using System.ComponentModel.DataAnnotations;

namespace FashionAPI.Models.Request
{
    public class UploadFileRequest
    {
        [Required]
        public IFormFile FileData { get; set; }
    }

    public class UploadMultipleFileRequest
    {
        public List<IFormFile> FileData { get; set; } = new List<IFormFile>(); // Hỗ trợ nhiều ảnh
    }
}
