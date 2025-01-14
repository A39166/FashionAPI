using FashionAPI.Models.BaseRequest;
using System.ComponentModel.DataAnnotations;

namespace FashionAPI.Models.Request
{
    public class ChangePasswordRequest 
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
