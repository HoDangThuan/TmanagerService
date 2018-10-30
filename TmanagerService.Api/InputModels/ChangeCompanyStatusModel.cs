using System.ComponentModel.DataAnnotations;

namespace TmanagerService.Api.InputModels
{
    public class ChangeCompanyStatusModel
    {
        [Required]
        public string CompanyId { get; set; }
    }
}
