using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models.Entities
{
    [Table("Notification")]
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotificationId { get; set; }
       
        [Required]
        [StringLength(500)] 
        public string Message { get; set; }

        [Required]
        public bool? IsRead { get; set; } = false; 

        [Required]
        public DateTime? DateSent { get; set; } = DateTime.Now;
    }
}
