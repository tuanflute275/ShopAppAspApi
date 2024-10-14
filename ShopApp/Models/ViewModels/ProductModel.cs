using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models.ViewModels
{
    public class ProductModel
    {
        [Required]
        public string ProductName { get; set; }
        public IFormFile ImageFile { get; set; }
        public string? OldImage { get; set; }
        [Range(0, double.MaxValue)]
        public double ProductPrice { get; set; }
        [Range(0, double.MaxValue)]
        public double ProductSalePrice { get; set; }
        public bool ProductStatus { get; set; }
        public int CategoryId { get; set; }
        public string ProductDescription { get; set; }
    }
}
