using System;
using System.Collections.Generic;

namespace TmanagerService.Api.Identity
{
    public class ListviewRequest
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string Address { get; set; }
        public double? Latlng_latitude { get; set; }
        public double? Latlng_longitude { get; set; }
        public DateTime? TimeBeginRequest { get; set; }
        public DateTime? TimeReceiveRequest { get; set; }
        public DateTime? TimeFinish { get; set; }
        public DateTime? TimeConfirm { get; set; }
        public List<string> PictureRequest { get; set; }
        //public string AreaWorkingId { get; set; }
        //public string AreaWorkingName { get; set; }
        public string SupervisorId { get; set; }
    }
}
