using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models.ViewModels
{
    public class OrderModel
    {
        [Required]
        public string OrderFullName { get; set; }
        [Required]

        public string OrderAddress { get; set; }
        [Required]

        public string OrderPhoneNumber { get; set; }
        [EmailAddress]

        public string? OrderEmail { get; set; }

        public string? OrderPaymentMethods { get; set; }

        public string? OrderStatusPayment { get; set; }

        public double? OrderAmount { get; set; }

        public string? OrderNote { get; set; }

        public int UserId { get; set; }

        public int? CouponId { get; set; }
    }
}
