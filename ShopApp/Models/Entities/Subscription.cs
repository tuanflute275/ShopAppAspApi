using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models.Entities
{
    //lưu trữ thông tin về việc đăng ký nhận tin tức hoặc dịch vụ từ hệ thống
    [Table("Subscription")]
    public class Subscription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubscriptionId { get; set; }

        // SubscriptionType: Loại hình đăng ký
        // (ví dụ: "Promotions" - Khuyến mại,
        // "Newsletters", hoặc - bản tin
        // "Premium Services" - dịch vụ cao cấp).
        [Required]
        [StringLength(50)]
        public string SubscriptionType { get; set; }

        [Required]
        public DateTime? CreateDate { get; set; }

        [Required]
        public DateTime? UpdateDate { get; set; }

        // Navigation properties
        [Column("UserId")]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
