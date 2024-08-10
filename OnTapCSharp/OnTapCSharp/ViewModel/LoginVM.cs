using System.ComponentModel.DataAnnotations;

namespace OnTapCSharp.ViewModel
{
    public class LoginVM
    {
        [Display(Name = "Tài khoản")]
        [Required(ErrorMessage = "*")]
        [MaxLength(20, ErrorMessage = "Tối đa 20 ký tự")]
        public string UserName { get; set; }
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
