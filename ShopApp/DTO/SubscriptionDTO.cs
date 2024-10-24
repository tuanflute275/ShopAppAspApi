namespace ShopApp.DTO
{
    public class SubscriptionDTO
    {
        public int SubscriptionId { get; set; }
        public string SubscriptionType { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? Username { get; set; }
    }
}
