namespace ShopApp.DTO
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSlug { get; set; }
        public string ProductImage { get; set; }
        public double ProductPrice { get; set; }
        public double ProductSalePrice { get; set; }
        public bool ProductStatus { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategorySlug { get; set; }
        public string ProductDescription { get; set; }
        public List<string> ProductImages { get; set; } = new List<string>();
        public List<ProductCommentDTO> ProductComments { get; set; } = new List<ProductCommentDTO>();
        public List<ProductDetailDTO> ProductDetails { get; set; } = new List<ProductDetailDTO>();

    }
}
