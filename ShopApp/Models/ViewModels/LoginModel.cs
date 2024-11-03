using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models.ViewModels
{
    public class LoginModel
    {
        public string? UsernameOrEmail { get; set; }
        [Required]
        [MinLength(5)]
        public string Password { get; set; }
    }
}
