using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models.Entities
{
    public class CouponOrder
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Coupon")]
        public int CouponId { get; set; }
        public virtual Coupon Coupon { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
    }
}
