using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models.ViewModels
{
    public class LoginModel
    {
        public string? UsernameOrEmail { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
