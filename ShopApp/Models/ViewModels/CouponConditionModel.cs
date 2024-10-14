using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models.ViewModels
{
    public class CouponConditionModel
    {
        [Required(ErrorMessage = "CouponId không được bỏ trống")]
        public int CouponId { get; set; }
        [Required(ErrorMessage = "Attribute không được bỏ trống")]
        public string Attribute { get; set; }
        [Required(ErrorMessage = "Operator không được bỏ trống")]
        public string Operator { get; set; }
        [Required(ErrorMessage = "Value không được bỏ trống")]
        public string Value { get; set; }
        public double DiscountAmount { get; set; }
    }
}
