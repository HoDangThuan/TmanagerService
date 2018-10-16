using System.ComponentModel.DataAnnotations;

namespace TmanagerService.Api.InputModels
{
    public class ChangeAreaUserModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Area { get; set; }
    }
}
