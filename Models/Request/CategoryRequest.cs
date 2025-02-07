using FashionAPI.Models.BaseRequest;

namespace ESDManagerApi.Models.Request
{
    public class CategoryRequest:BaseKeywordRequest
    {
        public string? Uuid { get; set; }
    }
}
