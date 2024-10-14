using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models.ViewModels
{
    public class CouponConditionModel
    {
        [Required]
        public int CouponId { get; set; }
        [Required]
        public string Attribute { get; set; }
        [Required]
        public string Operator { get; set; }
        [Required]
        public string Value { get; set; }
        public double DiscountAmount { get; set; }
    }
}
