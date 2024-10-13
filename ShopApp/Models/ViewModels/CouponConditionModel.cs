namespace ShopApp.Models.ViewModels
{
    public class CouponConditionModel
    {
        public int CouponId { get; set; }
        public string Attribute { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public double DiscountAmount { get; set; }
    }
}
