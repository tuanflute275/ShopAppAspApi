using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShopApp.Models.Entities
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("OrderId")]
        public int OrderId { get; set; }

        [Column("OrderFullName", TypeName = "nvarchar(200)")]
        public string OrderFullName { get; set; }

        [Column("OrderAddress", TypeName = "nvarchar(200)")]
        public string OrderAddress { get; set; }

        [Column("OrderPhoneNumber", TypeName = "nvarchar(15)")]
        [Phone]
        public string OrderPhoneNumber { get; set; }

        [Column("OrderEmail", TypeName = "nvarchar(200)")]
        [EmailAddress]
        public string OrderEmail { get; set; }

        [Column("OrderDate")]
        public DateTime OrderDate { get; set; }

        [Column("OrderPaymentMethods", TypeName = "nvarchar(100)")]
        public string? OrderPaymentMethods { get; set; }

        [Column("OrderStatusPayment", TypeName = "nvarchar(100)")]
        public string? OrderStatusPayment { get; set; }

        [Column("OrderStatus")]
        public bool OrderStatus { get; set; }

        [Column("OrderAmount")]
        [Range(0, double.MaxValue)]
        public double OrderAmount { get; set; }

        [Column("OrderNote", TypeName = "ntext")]
        public string OrderNote { get; set; }

        [Column]
        public int UserId { get; set; }
        [Column] 
        public int? CouponId { get; set; }

        [ForeignKey("UserId")]
        [JsonIgnore]
        public virtual User User { get; set; }

        [ForeignKey("CouponId")]
        [JsonIgnore]
        public virtual Coupon Coupon { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    }
}
