using System;
using System.Collections.Generic;
using TmanagerService.Core.Entities;

namespace TmanagerService.Api.Identity
{
    public class InformationUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        //public List<AreaWorkingValues> ListAreaWorking { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Note { get; set; }
        public string Avatar { get; set; }
        public bool IsEnabled { get; set; }
    }
}
