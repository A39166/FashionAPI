using FashionAPI.Models.BaseRequest;
using FashionAPI.Models.DataInfo;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FashionAPI.Models.Request
{
    public class UpdateUserRequest : UuidRequest
    {
        public string Fullname { get; set; }
        public sbyte Gender { get; set; }
        public DateOnly Birthday { get; set; }
        public string? PhoneNumber { get; set; }
    }
    public class UpdateUserClientRequest : UuidRequest
    {

        public string Fullname { get; set; }

        public sbyte Gender { get; set; }
        public DateOnly Birthday { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Path { get; set; }
    }
}
