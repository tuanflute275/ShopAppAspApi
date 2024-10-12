using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApp.Models.ViewModels
{
    public class CategoryModel
    {
        public string CategoryName { get; set; }
        public bool CategoryStatus { get; set; }
    }
}
