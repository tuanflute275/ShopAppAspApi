namespace ShopApp.DTO
{
    public class OrderDetailDTO
    {
        public int OrderDetailId { get; set; }
        public string ProductName { get; set; }
        public string ProductSlug { get; set; }
        public string ProductImage { get; set; }
        public double ProductPrice { get; set; }
        public string CategoryName { get; set; }
        public bool ProductStatus { get; set; }
        public int Quantity { get; set; }
        public double TotalMoney { get; set; }
    }
}
