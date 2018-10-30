using System.ComponentModel.DataAnnotations;

namespace TmanagerService.Api.InputModels
{
    public class ForgotPasswordModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}