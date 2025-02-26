using FashionAPI.Models.BaseRequest;
using System.ComponentModel;

namespace FashionAPI.Models.Request
{
    public class PageListProductClientRequest : DpsPagingParamBase
    {
        public string? CategoryUuid { get; set; }
        public string? ColorUuid { get; set; }
        [DefaultValue(1)]
        public sbyte? Sorted {  get; set; } // 1 - Thấp đến cao, 2 - Cao đến thấp


    }
}
