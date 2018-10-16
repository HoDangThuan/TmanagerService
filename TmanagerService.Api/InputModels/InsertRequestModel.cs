using System.ComponentModel.DataAnnotations;

namespace TmanagerService.Api.InpuModels
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
        public string PictureRequest { get; set; }

        //public string SupervisorId { get; set; }
        //public ApplicationUser Supervisor { get; set; }

        public string Note { get; set; }
    }
}
