using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models.ViewModels
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }
    }
}
