using FashionAPI.Models.BaseRequest;

namespace FashionAPI.Models.Request
{
    public class FindProvinceRequest : BaseKeywordRequest
    {
        public string? IdParent { get; set; }
    }
}
