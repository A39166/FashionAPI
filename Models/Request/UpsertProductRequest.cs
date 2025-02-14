using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FashionAPI.Models.BaseRequest;

namespace FashionAPI.Models.Request
{
    public class UpsertProductRequest: UuidRequest
    {
        public string CatUuid { get; set; }
        public string ColorUuid { get; set; }
        public string Code { get; set; }
        public string ProductName { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public List<ProVariant> Variants { get; set; }
        public List<string>? ImagesPath { get; set; }

    }
    public class ProVariant
    {
        public string SizeUuid { get; set; }
        public int Stock { get; set; }

    }
}
