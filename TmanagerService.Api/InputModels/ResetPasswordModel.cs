using System.ComponentModel.DataAnnotations;

namespace TmanagerService.Api.InputModels
{
    public class ResetPasswordModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string PasswordConfirm { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
