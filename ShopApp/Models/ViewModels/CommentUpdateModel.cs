using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models.ViewModels
{
    public class CommentUpdateModel
    {
        [EmailAddress]
        [Column("Email", TypeName = "nvarchar(200)")]
        public string Email { get; set; }
        [Column("Name", TypeName = "nvarchar(200)")]
        public string Name { get; set; }
        [Column("Message", TypeName = "ntext")]
        public string Message { get; set; }
    }
}
