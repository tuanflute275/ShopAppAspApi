using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApp.Models.Entities
{
    [Table("UserRole")]
    public class UserRole
    {
        [Key]
        [Column]
        [Required]
        [StringLength(255)]
        public int UserId { get; set; }

        [Column]
        [Required]
        [StringLength(255)]
        public int RoleId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
    }
}
