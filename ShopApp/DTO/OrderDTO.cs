namespace ShopApp.DTO
{
    public class OrderDTO
    {
        public int OrderId { get; set; }

        public string OrderFullName { get; set; }

        public string OrderAddress { get; set; }

        public string OrderPhoneNumber { get; set; }

        public string? OrderEmail { get; set; }

        public DateTime? OrderDate { get; set; }

        public string? OrderPaymentMethods { get; set; }

        public string? OrderStatusPayment { get; set; }

        public int? OrderStatus { get; set; }

        public int? OrderQuantity { get; set; }

        public double? OrderAmount { get; set; }

        public string? OrderNote { get; set; }

        public UserDTO User { get; set; } = new UserDTO();
        public List<CouponDTO> Coupons { get; set; } = new List<CouponDTO>();
        public List<OrderDetailDTO> OrderDetails { get; set; } = new List<OrderDetailDTO>();
    }
}
