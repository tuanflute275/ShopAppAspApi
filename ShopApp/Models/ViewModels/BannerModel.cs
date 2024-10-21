namespace ShopApp.Models.ViewModels
{
    public class BannerModel
    {
        public string Title { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? OldImage { get; set; }
    }
}
