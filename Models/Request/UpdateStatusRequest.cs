using FashionAPI.Models.BaseRequest;

namespace FashionAPI.Models.Request
{
    public class UpdateStatusRequest : UuidRequest
    {
        public sbyte Status { get; set; }
    }
}
