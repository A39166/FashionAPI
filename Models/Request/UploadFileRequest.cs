using System.ComponentModel.DataAnnotations;

namespace FashionAPI.Models.Request
{
    public class UploadFileRequest
    {
        [Required]
        public IFormFile FileData { get; set; }
        public sbyte Type { get; set; }
    }
}
