namespace ShopApp.DTO
{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<ProductDTO> Products { get; set; }
    }
}
