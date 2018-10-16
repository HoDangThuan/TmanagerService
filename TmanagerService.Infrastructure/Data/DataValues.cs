using System;
using System.Collections.Generic;
using System.Globalization;
using TmanagerService.Core.Entities;
using TmanagerService.Core.Enums;
using TmanagerService.Core.Extensions;

namespace TmanagerService.Infrastructure.Data
{
    public class DataValues
    {
        public const string default_admin_password = "Abc123!@#";
        public const string default_user_password = "Abc123!@#";
        static readonly CultureInfo ci = CultureInfo.InvariantCulture;
        public static IEnumerable<ApplicationUser> AdminData()
        {
            return new List<ApplicationUser>()
            {
                new ApplicationUser { UserName = "Admin01", Email = "NguyenVanQuan@yahoo.com", SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = "Quản", LastName = "Nguyễn Văn", EmailConfirmed = true, Address = "33 Nguyễn Thị Thập, Đà Nẵng",
                    Position = Role.Admin.ToDescription(), IsEnabled = true, Note = "Admin", Gender = Gender.Male.ToDescription(),
                    DateOfBirth = DateTime.ParseExact("25/12/1965", "dd/MM/yyyy", ci), PhoneNumber =  "0935846275" }
            };
        }
        public static IEnumerable<ApplicationUser> SupervisorData()
        {
            return new List<ApplicationUser>()
            {
                new ApplicationUser { UserName = "HoDangThuan", Email = "HoDangThuan@yahoo.com", SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = "Thuần", LastName = "Hồ Đăng", EmailConfirmed = true, Address = "33 Tô Hiệu, Huế",
                    Position = Role.Supervisor.ToDescription(), IsEnabled = true, Note = "Supervisor", Gender = Gender.Male.ToDescription(),
                    DateOfBirth = DateTime.ParseExact("18/08/1989", "dd/MM/yyyy", ci), PhoneNumber =  "0965784235",
                    Area = "Hải Châu, Đà Nẵng" }
            };
        }
        public static IEnumerable<ApplicationUser> RepairPersonData()
        {
            return new List<ApplicationUser>()
            {
                new ApplicationUser { UserName = "NguyenVanHien", Email = "NguyenVanHien@yahoo.com", SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = "Hiền", LastName = "Nguyễn Văn", EmailConfirmed = true, Address = "73 Tô Thất Tùng, Huế",
                    Position = Role.RepairPerson.ToDescription(), IsEnabled = true, Note = "Repair Person", Gender = Gender.Female.ToDescription(),
                    DateOfBirth = DateTime.ParseExact("20/10/1995", "dd/MM/yyyy", ci), PhoneNumber =  "0965847596",
                    Area = "Hải Châu, Đà Nẵng" }
            };
        }
    }
}
