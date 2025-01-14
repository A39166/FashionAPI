using FashionAPI.Models.BaseRequest;

namespace FashionAPI.Models.Request
{
    public class EnterOtpReqDTO 
    {
        public string Otp { get; set; }
        public string Email { get; set; }
    }
}
