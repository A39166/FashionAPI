using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FashionAPI.Models.BaseRequest;

namespace FashionAPI.Models.Request
{
    public class UpsertSizeRequest : UuidRequest
    {
        public string SizeName { get; set; }
    }
}
