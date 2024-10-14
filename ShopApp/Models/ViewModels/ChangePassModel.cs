using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models.ViewModels
{
    public class ChangePassModel
    {
        [Required]
        [MinLength(6)]
        public string OldPassword { get; set; }

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }

        [Required]
        [MinLength(6)]
        public string ConfirmPassword { get; set; }
    }
}
