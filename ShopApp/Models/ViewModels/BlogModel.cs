using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models.ViewModels
{
    public class BlogModel
    {
        [Required]
        public string BlogTitle { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? OldImage { get; set; }
        public string? BlogDescription { get; set; }
        public int UserId { get; set; }
    }
}
