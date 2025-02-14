namespace FashionAPI.Models.Request
{
    public class AddToCartRequest
    {
        public string ProductUuid { get; set; }
        public string SizeUuid { get; set; }
        public int quantity { get; set; }
        
    }
}
