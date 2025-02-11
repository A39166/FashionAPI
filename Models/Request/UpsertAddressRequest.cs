using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FashionAPI.Models.BaseRequest;

namespace FashionAPI.Models.Request
{
    public class UpsertAddressRequest : UuidRequest
    {
        
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string? Matp { get; set; }

        public string? Maqh { get; set; }

        public string? Xaid { get; set; }

    }
}
