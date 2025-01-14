using FashionAPI.Models.BaseRequest;

namespace FashionAPI.Models.BaseRequest
{
    public class BaseKeywordPageRequest : DpsPagingParamBase
    {
        public string? Keyword { get; set; }

        public sbyte? Status { get; set; }
    }
}
