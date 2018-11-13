using System;
using System.Collections.Generic;

namespace TmanagerService.Api.Identity
{
    public class ReturnRequest
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string Address { get; set; }
        public double? Latlng_latitude { get; set; }
        public double? Latlng_longitude { get; set; }
        public string TimeBeginRequest { get; set; }
        public string TimeReceiveRequest { get; set; }
        public string TimeFinish { get; set; }
        public string TimeConfirm { get; set; }
        public List<string> PictureRequest { get; set; }
        public List<string> PictureFinish { get; set; }
        public string Status { get; set; }
        public string SupervisorId { get; set; }
        public string SupervisorName { get; set; }
        public string RepairPersonId { get; set; }
        public string RepairPersonName { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Note { get; set; }
    }
}
