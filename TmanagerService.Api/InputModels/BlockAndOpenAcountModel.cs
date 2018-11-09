using System.ComponentModel.DataAnnotations;

namespace TmanagerService.Api.InputModels
{
    public class BlockAndOpenAcountModel
    {
        [Required]
        public string UserId { get; set; }
    }
}
