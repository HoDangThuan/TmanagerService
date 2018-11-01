using System.ComponentModel.DataAnnotations;

namespace TmanagerService.Api.InputModels
{
    public class DisableAccountModel
    {
        [Required]
        public string UserId { get; set; }
    }
}
