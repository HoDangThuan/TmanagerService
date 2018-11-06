using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TmanagerService.Api.InputModels
{
    public class AddCompanyModel
    {
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public IFormFile Logo { get; set; }
        [Required]
        public bool IsDepartmentOfConstruction { get; set; }
    }
}
