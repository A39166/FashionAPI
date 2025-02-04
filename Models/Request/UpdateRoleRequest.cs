using FashionAPI.Models.BaseRequest;

namespace FashionAPI.Models.Request
{
    public class UpdateRoleRequest : UuidRequest
    {
        public sbyte Role { get; set; }
    }
}
