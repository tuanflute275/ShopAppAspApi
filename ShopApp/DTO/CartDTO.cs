namespace ShopApp.DTO
{
    public class CartDTO
    {
        public int CartId { get; set; }
        public int Quantity { get; set; }
        public double? TotalAmount { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public string? UserPhoneNumber { get; set; }
        public string? UserAddress { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSlug { get; set; }
        public string ProductImage { get; set; }
        public double ProductPrice { get; set; }
        public bool ProductStatus { get; set; }
    }
}
