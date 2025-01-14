using FashionAPI.Models.BaseRequest;

namespace FashionAPI.Models.Request
{
    public class ChangePasswordForget 
    {
        public string Otp {  get; set; }
        public string Email { get; set; }
        public string NewPassword { get; set; }

    }
}
