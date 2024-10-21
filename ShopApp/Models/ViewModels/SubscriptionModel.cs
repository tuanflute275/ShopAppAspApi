namespace ShopApp.Models.ViewModels
{
    public class SubscriptionModel
    {
        public string SubscriptionType { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int UserId { get; set; }
    }
}
