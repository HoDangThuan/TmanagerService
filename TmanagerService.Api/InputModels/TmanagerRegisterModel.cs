using System.ComponentModel.DataAnnotations;

namespace TmanagerService.Api.InpuModels
{
    public class TmanagerRegisterModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public byte Gender { get; set; }
        [Required]
        public string CompanyId { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
    }
}
