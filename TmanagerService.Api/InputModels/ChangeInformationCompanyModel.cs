using System.ComponentModel.DataAnnotations;

namespace TmanagerService.Api.InputModels
{
    public class ChangeInformationCompanyModel
    {
        [Required]
        public string CompanyId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
    }
}
