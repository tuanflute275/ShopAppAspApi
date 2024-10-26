namespace ShopApp.Models.ViewModels
{
    public class ProductImageUpdateModel
    {
        public int ImageId { get; set; }
        public int ProductId { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
