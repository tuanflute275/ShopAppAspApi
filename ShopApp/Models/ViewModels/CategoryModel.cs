using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApp.Models.ViewModels
{
    public class CategoryModel
    {
        [Required]
        public string CategoryName { get; set; }
        public bool CategoryStatus { get; set; }
    }
}
