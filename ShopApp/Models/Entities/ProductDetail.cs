using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApp.Models.Entities
{
    [Table("ProductDetail")]
    public class ProductDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ProductDetailId")]
        public int ProductDetailId { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public string Color { get; set; }
        [Column(TypeName = "nvarchar(10)")]
        public string Size { get; set; }
        [Column]
        public int Quantity { get; set; }

        [Column("ProductId")]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [Column("CreateDate")]
        public DateTime? CreateDate { get; set; } = DateTime.Now;

        [Column("UpdateDate")]
        public DateTime? UpdateDate { get; set; } = DateTime.Now;
    }
}
