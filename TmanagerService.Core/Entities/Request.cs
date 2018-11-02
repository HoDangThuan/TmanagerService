using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TmanagerService.Core.Entities
{
    public class Request
    {
        [Key]
        [StringLength(50)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string Content { get; set; }

        public string Address { get; set; }

        public double? Latlng_latitude { get; set; }

        public double? Latlng_longitude { get; set; }

        public DateTime? TimeBeginRequest { get; set; }

        public DateTime? TimeReceiveRequest { get; set; }

        public DateTime? TimeFinish { get; set; }

        public DateTime? TimeConfirm { get; set; }

        public string PictureRequest { get; set; }

        public string PictureFinish { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public string SupervisorId { get; set; }
        public virtual ApplicationUser Supervisor { get; set; }

        public string CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public string RepairPersonId { get; set; }
        public virtual ApplicationUser RepairPerson { get; set; }

        public string ReportUserId { get; set; }
        public virtual ApplicationUser ReportUser { get; set; }

        public string ReportContent { get; set; }

        public string Note { get; set; }
    }
}