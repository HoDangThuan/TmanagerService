using System;
using System.ComponentModel.DataAnnotations;

namespace TmanagerService.Core.Entities
{
    public class Request
    {
        [Key]
        public string Id { get; set; }
        public string Content { get; set; }
        public string Address { get; set; }
        public double Latlng_latitude { get; set; }
        public double Latlng_longitude { get; set; }
        public DateTime TimeBeginRequest { get; set; }
        public DateTime TimeReceiveRequest { get; set; }
        public DateTime TimeFinish { get; set; }
        public DateTime TimeConfirm { get; set; }
        public string PictureRequest { get; set; }
        public string PictureFinish { get; set; }
        public string Status { get; set; }

        public string SupervisorId { get; set; }
        public virtual ApplicationUser Supervisor { get; set; }
        
        public string RepairPersonId { get; set; }
        public virtual ApplicationUser RepairPerson { get; set; }

        public string Note { get; set; }
    }
}
