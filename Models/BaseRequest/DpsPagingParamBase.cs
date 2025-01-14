using System.ComponentModel;

namespace FashionAPI.Models.BaseRequest
{
    public class DpsPagingParamBase
    {
        [DefaultValue(10)]
        public int PageSize { get; set; } = 10;
        [DefaultValue(1)]
        public int Page { get; set; } = 1;
    }
}
