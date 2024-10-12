using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApp.Models.Entities
{
    [Table("Role")]
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Column(TypeName = "nvarchar(200)")]
        public string RoleName { get; set; }

        [Column(TypeName = "ntext")]
        public string RoleDescription { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
