using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApp.Models.ViewModels
{
    public class CategoryModel
    {
        [Required(ErrorMessage = "CategoryName không được bỏ trống")]
        public string CategoryName { get; set; }
        public bool CategoryStatus { get; set; }
    }
}
