using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TmanagerService.Api.InputModels
{
    public class RepairPersonFinishModel
    {
        [Required]
        public string RequestId { get; set; }
        [Required]
        public List<IFormFile> ListPictureFinish { get; set; }
    }
}
