using FashionAPI.Databases.FashionDB;
using FashionAPI.Models.BaseRequest;
using System.ComponentModel;

namespace FashionAPI.Models.DataInfo
{
    public class PageListAddressDTO
    {
        public string Uuid { get; set; }
        public string Fullname { get; set; } = null!;
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime TimeCreated { get; set; }
        public sbyte Status { get; set; }

}
}
