using FashionAPI.Databases.FashionDB;
using FashionAPI.Models.BaseRequest;
using System.ComponentModel;

namespace FashionAPI.Models.DataInfo
{
    public class PageListUserDTO : DpsPagingParamBase
    {
        public string Uuid { get; set; }
        public string Fullname { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = null!;
        public string? Path { get; set; }
        public sbyte Role { get; set; }
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }

}
}
