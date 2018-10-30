using System.ComponentModel.DataAnnotations;

namespace TmanagerService.Api.InputModels
{
    public class SupervisorConfirmModel
    {
        [Required]
        public string RequestId { get; set; }
    }
}