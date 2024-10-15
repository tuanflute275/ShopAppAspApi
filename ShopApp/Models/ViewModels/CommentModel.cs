using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApp.Models.ViewModels
{
    public class CommentModel
    {
        [EmailAddress]
        [Column("Email", TypeName = "nvarchar(200)")]
        public string Email { get; set; }
        [Column("Name", TypeName = "nvarchar(200)")]
        public string Name { get; set; }
        [Column("Message", TypeName = "ntext")]
        public string Message { get; set; }
        public int UserId { get; set; }
        public int IdPost { get; set; }
    }
}
