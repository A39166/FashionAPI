using FashionAPI.Models.Request;

namespace FashionAPI.Models.DataInfo
{
    public class CartDTO : BaseDTO
    {
        public List<CartItemDTO> Items { get; set; }
        public sbyte Status { get; set; }
    }
    public class CartItemDTO
    {
        public List<Variant> Variants { get; set; }
    }
}
