using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FashionAPI.Models.BaseRequest;

namespace FashionAPI.Models.Request
{
    public class UpsertCategoryRequest : UuidRequest
    {
        public string? ParentUuid { get; set; }
        public string CategoryName { get; set; }

    }
}
