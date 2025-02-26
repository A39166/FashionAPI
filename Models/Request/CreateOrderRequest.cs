namespace FashionAPI.Models.Request
{
    public class CreateOrderRequest
    {
        public List<ProductForOrder> Product {  get; set; }
        public string AddressUuid { get; set; }
        public double TotalPrice { get; set; }
        public string Note { get; set; }
    }

    public class ProductForOrder
    {
        public string ProductUuid { get; set; }
        public string SizeUuid { get; set; }
        public double Price { get; set; }
        public int quantity { get; set; }

    }
}
