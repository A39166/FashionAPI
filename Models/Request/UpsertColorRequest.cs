using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FashionAPI.Models.BaseRequest;

namespace FashionAPI.Models.Request
{
    public class UpsertColorRequest: UuidRequest
    {
        public string ColorName { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
