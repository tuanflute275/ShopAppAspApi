using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ShopApp.Models.Entities
{
    [Table("User")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Column(TypeName = "nvarchar(200)")]
        public string UserName { get; set; }

        [Required]
        [StringLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string UserFullName { get; set; }

        [StringLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string? UserAvatar { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string UserEmail { get; set; }

        [Required]
        [StringLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string UserPassword { get; set; }

        [Phone]
        [StringLength(15)]
        [Column(TypeName = "nvarchar(15)")]
        public string? UserPhoneNumber { get; set; }

        [StringLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string? UserAddress { get; set; }

        [Column]
        public bool UserGender { get; set; } = true;

        [Column]
        public bool UserActive { get; set; } = true;

        [Range(0, int.MaxValue)]
        [Column]
        public int? UserCount { get; set; }

        [Column]
        public DateTime? UserCurrentTime { get; set; } 

        [Column]
        public DateTime? UserUnlockTime { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }

        [JsonIgnore]
        public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
    }
}
