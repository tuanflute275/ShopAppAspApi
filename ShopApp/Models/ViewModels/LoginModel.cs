using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models.ViewModels
{
    public class LoginModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email không được bỏ trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        [MinLength(6, ErrorMessage = "Tối thiểu 6 kí tự")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
