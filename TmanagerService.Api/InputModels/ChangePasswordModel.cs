using System.ComponentModel.DataAnnotations;

namespace TmanagerService.Api.InpuModels
{
    public class ChangePasswordModel
    {
        //[Required]
        //public string UserName { get; set; }
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string NewPasswordConfirm { get; set; }
    }
}
