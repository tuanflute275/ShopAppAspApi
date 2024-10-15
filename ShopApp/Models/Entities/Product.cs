using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShopApp.Models.Entities
{
    [Table("Product")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ProductId")]
        public int ProductId { get; set; }

        [StringLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string ProductName { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string ProductSlug { get; set; }

        [StringLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string ProductImage { get; set; }

        [Range(0, double.MaxValue)]
        public double ProductPrice { get; set; }

        [Range(0, double.MaxValue)]
        public double ProductSalePrice { get; set; }

        [Column]
        public int CategoryId { get; set; }

        [Column(TypeName = "ntext")]
        public string ProductDescription { get; set; }

        [Column]
        public bool ProductStatus { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        [JsonIgnore]
        public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public virtual ICollection<ProductDetail> ProductDetails { get; set; } = new List<ProductDetail>();
        public virtual ICollection<ProductComment> ProductComments { get; set; } = new List<ProductComment>();
    }
}
