using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TmanagerService.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(15)]
        public string FirstName { get; set; }

        [StringLength(150)]
        public string LastName { get; set; }

        public bool IsEnabled { get; set; }

        public string Token { get; set; }

        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }

        [StringLength(150)]
        public string Address { get; set; }

        [StringLength(6)]
        public string Gender { get; set; }

        public string Role { get; set; }

        public string AdminId { get; set; }

        public string CompanyId { get; set; }
        public virtual Company Company { get; set; }

        //public string ListAreaWorkingId { get; set; }

        public string Note { get; set; }
    }
}