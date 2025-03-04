using FashionAPI.Models.BaseRequest;
using System.ComponentModel;

namespace FashionAPI.Models.Request
{
    public class PageListOrderAdminRequest : BaseKeywordPageRequest
    {
        public sbyte State {  get; set; }
    }
}
