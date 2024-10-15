using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApp.Models.Entities
{
    [Table("ProductComment")]
    public class ProductComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ProductCommentId")]
        public int ProductCommentId { get; set; }

        [EmailAddress]
        [Column("Email", TypeName = "nvarchar(200)")]
        public string Email { get; set; }
        [Column("Name", TypeName = "nvarchar(200)")]
        public string Name { get; set; }
        [Column("Message", TypeName = "ntext")]
        public string Message { get; set; }

        [Column("CreateDate")]
        public DateTime? CreateDate { get; set; } = DateTime.Now;

        [Column("UpdateDate")]
        public DateTime? UpdateDate { get; set; } = DateTime.Now;

        [Column("UserId")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Column("ProductId")]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
