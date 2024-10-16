namespace ShopApp.DTO
{
    public class CouponDTO
    {
        public int CouponId { get; set; }
        public string Code { get; set; }
        public int? Percent { get; set; }
        public string? Description { get; set; }
        public bool Active { get; set; }
        public List<CouponConditionDTO> CouponConditions { get; set; } = new List<CouponConditionDTO>();
    }
}
