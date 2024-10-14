using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApp.Models.Entities
{
    [Table("BlogComment")]
    public class BlogComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("BlogCommentId")]
        public int BlogCommentId { get; set; }

        [EmailAddress]
        [Column("Email")]
        public string Email { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("Message")]
        public string Message { get; set; }

        [Column("CreateDate")]
        public DateTime? CreateDate { get; set; } = DateTime.Now;

        [Column("UpdateDate")]
        public DateTime? UpdateDate { get; set; } = DateTime.Now;

        [Column("UserId")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Column("BlogId")]
        public int BlogId { get; set; }

        [ForeignKey("BlogId")]
        public virtual Blog Blog { get; set; }
    }
}
