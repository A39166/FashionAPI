using FashionAPI.Models.BaseRequest;

namespace FashionAPI.Models.Request
{
    public class PageListProductClientRequest : BaseKeywordPageRequest
    {
        public string? CategoryUuid { get; set; }
        public string? ColorUuid { get; set; }

        
    }
}
