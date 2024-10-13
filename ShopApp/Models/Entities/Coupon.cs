using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShopApp.Models.Entities
{
    [Table("Coupon")]
    public class Coupon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        [Required]
        public string Code { get; set; }

        [Range(0, 100)] 
        public int? Percent { get; set; }

        [Column(TypeName = "ntext")]
        public string? Description { get; set; }

        [Column]
        public bool Active { get; set; } = true;

        [JsonIgnore]
        public virtual ICollection<CouponCondition> CouponConditions { get; set; } = new List<CouponCondition>();
    }
}
