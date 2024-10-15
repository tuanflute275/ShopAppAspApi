using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShopApp.Models.Entities
{
    [Table("Blog")]
    public class Blog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("BlogId")]
        public int BlogId { get; set; }

        [Column("BlogImage", TypeName = "nvarchar(200)")]
        public string? BlogImage { get; set; }

        [Column("BlogTitle", TypeName = "nvarchar(200)")]
        [Required]
        [StringLength(200)]
        public string BlogTitle { get; set; }

        [Column("BlogSlug", TypeName = "nvarchar(200)")]
        public string? BlogSlug { get; set; }

        [Column("BlogDescription", TypeName = "ntext")]
        [Required]
        public string? BlogDescription { get; set; }

        [Column("CreateDate")]
        public DateTime? CreateDate { get; set; } = DateTime.Now; 

        [Column("UpdateDate")]
        public DateTime? UpdateDate { get; set; } = DateTime.Now; 

        [Column("UserId")]
        public int UserId { get; set; } 

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public virtual ICollection<BlogComment> BlogComments { get; set; } = new List<BlogComment>();   
    }
}
