using FashionAPI.Databases.FashionDB;
using System.ComponentModel;

namespace FashionAPI.Models.DataInfo
{
    public class UserDTO : BaseDTO
    {
        public string Fullname { get; set; } = null!;
        public sbyte Gender { get; set; }
        public DateOnly Birthday { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = null!;
        public string? Path { get; set; }
        public sbyte Role { get; set; }
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }

    }
    public class UserClientDTO : BaseDTO
    {
        public string Fullname { get; set; } = null!;
        public sbyte Gender { get; set; }
        public DateOnly Birthday { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = null!;
        public string? Path { get; set; }
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }

    }
}
