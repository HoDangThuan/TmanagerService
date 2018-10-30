using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TmanagerService.Api.InputModels
{
    public class InsertRequestModel
    {
        [Required]
        public string Content { get; set; }
        [Required]
        public string Address { get; set; }
        public double Latlng_longitude { get; set; }
        public double Latlng_latitude { get; set; }
        [Required]
        public string CompanyId { get; set; }
        [Required]
        public List<IFormFile> PictureRequest { get; set; }
    }
}
