using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TmanagerService.Api.InputModels
{
    public class ChangeInformationCompanyModel
    {
        [Required]
        public string CompanyId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public IFormFile Logo { get; set; }
    }
}
