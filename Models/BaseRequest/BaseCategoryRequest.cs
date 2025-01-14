using FashionAPI.Models.BaseRequest;
using System.ComponentModel.DataAnnotations;

namespace FashionAPI.Models.BaseRequest
{
    public class BaseCategoryRequest : BaseKeywordRequest
    {

        public string? Uuid { get; set; }
    }
}
