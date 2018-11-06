using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TmanagerService.Core.Entities;
using TmanagerService.Core.Enums;
using TmanagerService.Core.Extensions;

namespace TmanagerService.Infrastructure.Data
{
    public class DataValues
    {
        public const string default_admin_password = "Abc123!@#";
        public const string default_user_password = "Abc123!@#";
        public const string adminName01 = "Admin01";
        public const string areaWorkingName01 = "Hải Châu, Đà Nẵng";
        public const string companyName01 = "Công ty xây dựng con vịt cồ";
        public const string DepartmentOfConstruction = "Sở xây dựng TP Đà Nẵng";
        static readonly CultureInfo ci = CultureInfo.InvariantCulture;
        
        public static IEnumerable<ApplicationUser> AdminData(TmanagerServiceContext dbContext)
        {
            return new List<ApplicationUser>()
            {
                new ApplicationUser { UserName = adminName01, Email = "WhoAmITmanager@gmail.com", SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = "Quản", LastName = "Nguyễn Văn", EmailConfirmed = true, Address = "33 Nguyễn Thị Thập, Đà Nẵng",
                    Role = RoleValues.Admin.ToDescription(), IsEnabled = true, Note = "Admin", Gender = Gender.Male.ToDescription(),
                    DateOfBirth = DateTime.ParseExact("25/12/1965", "dd/MM/yyyy", ci), PhoneNumber =  "0935846275" },
                new ApplicationUser { UserName = "Admin02", Email = "TranThiTri@gmail.com", SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = "Trị", LastName = "Trần Thị", EmailConfirmed = true, Address = "63 Nguyễn Thị Thập, Đà Nẵng",
                    Role = RoleValues.Admin.ToDescription(), IsEnabled = true, Note = "Admin", Gender = Gender.Female.ToDescription(),
                    DateOfBirth = DateTime.ParseExact("25/12/1967", "dd/MM/yyyy", ci), PhoneNumber =  "0968547213" }
            };
        }

        public static async Task<IEnumerable<Company>> CompanyDataAsync(TmanagerServiceContext dbContext, UserManager<ApplicationUser> userManager)
        {
            string adminId = (await userManager.FindByNameAsync(AdminData(dbContext).ToArray().FirstOrDefault().UserName)).Id;

            return new List<Company>()
            {
                new Company { AdminId = adminId, Name = companyName01, Address = "48 Cao Thắng, Đà Nẵng", Status = true, IsDepartmentOfConstruction = true,
                    Logo = "http://res.cloudinary.com/dj9j7j4sf/image/upload/v1541472243/cj8pkzjrj41abqcfo0kl.png" },
                new Company { AdminId = adminId, Name = "Công ty xây dựng 2", Address = "138 Đống Đa, Đà Nẵng", Status = true, IsDepartmentOfConstruction = false,
                    Logo = "http://res.cloudinary.com/dj9j7j4sf/image/upload/v1541472244/uoslpdsifawv5gdzeor5.png" },
                new Company { AdminId = adminId, Name = "Công ty xây dựng 3", Address = "55 Nguyễn Du, Đà Nẵng", Status = true, IsDepartmentOfConstruction = false,
                    Logo = "http://res.cloudinary.com/dj9j7j4sf/image/upload/v1541472245/klaupqhjuta8bsfjoygu.png" },
                new Company { AdminId = adminId, Name = DepartmentOfConstruction, Address = "Tầng 12, tầng 13 - Trung tâm hành chính - 24 Trần Phú, quận Hải Châu, thành phố Đà Nẵng", Status = true, IsDepartmentOfConstruction = false,
                    Logo = "http://res.cloudinary.com/dj9j7j4sf/image/upload/v1541472603/fyx5d6frnnb2u5mvlvjp.jpg" }
            };
        }

        //public static async Task<IEnumerable<AreaWorking>> AreaWorkingDataAsync(TmanagerServiceContext dbContext, UserManager<ApplicationUser> userManager)
        //{
        //    ApplicationUser admin = await userManager.FindByNameAsync(AdminData(dbContext).ToArray().FirstOrDefault().UserName);

        //    return new List<AreaWorking>()
        //    {
        //        new AreaWorking { AdminId = admin.Id, AreaName = areaWorkingName01, Status = true},
        //        new AreaWorking { AdminId = admin.Id, AreaName = "Cẩm Lệ, Đà Nẵng", Status = true},
        //        new AreaWorking { AdminId = admin.Id, AreaName = "Sơn Trà, Đà Nẵng", Status = true},
        //        new AreaWorking { AdminId = admin.Id, AreaName = "Liên Chiểu, Đà Nẵng", Status = false}
        //    };
        //}

        public static async Task<IEnumerable<ApplicationUser>> SupervisorDataAsync(TmanagerServiceContext dbContext, UserManager<ApplicationUser> userManager)
        {
            ApplicationUser admin = await userManager.FindByNameAsync(AdminData(dbContext).ToArray().FirstOrDefault().UserName);
            //AreaWorking[] areaWorkingData = dbContext.AreaWorkings.ToArray();
            Company departmentOfConstruction = dbContext.Companys.FirstOrDefault(c => c.Name == DepartmentOfConstruction);
            return new List<ApplicationUser>()
            {
                new ApplicationUser { UserName = "HoDangThuan", Email = "HoDangThuan@yahoo.com", SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = "Thuần", LastName = "Hồ Đăng", EmailConfirmed = true, Address = "33 Tô Hiệu, Huế",
                    Role = RoleValues.Supervisor.ToDescription(), IsEnabled = true, Note = "Supervisor", Gender = Gender.Male.ToDescription(),
                    DateOfBirth = DateTime.ParseExact("18/08/1989", "dd/MM/yyyy", ci), PhoneNumber =  "0965784235",
                    AdminId = admin.Id, Company = departmentOfConstruction, },
                new ApplicationUser { UserName = "NguyenNgocTan", Email = "NguyenNgocTan@yahoo.com", SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = "Tấn", LastName = "Nguyễn Ngọc", EmailConfirmed = true, Address = "73 Tô Hiệu, Huế",
                    Role = RoleValues.Supervisor.ToDescription(), IsEnabled = true, Note = "Supervisor", Gender = Gender.Female.ToDescription(),
                    DateOfBirth = DateTime.ParseExact("18/08/1995", "dd/MM/yyyy", ci), PhoneNumber =  "0986523475",
                    AdminId = admin.Id, Company = departmentOfConstruction, }
            };
        }
        public static async Task<IEnumerable<ApplicationUser>> RepairPersonDataAsync(TmanagerServiceContext dbContext, UserManager<ApplicationUser> userManager)
        {
            ApplicationUser admin = await userManager.FindByNameAsync(AdminData(dbContext).ToArray().FirstOrDefault().UserName);            
            Company company = dbContext.Companys.FirstOrDefault(c => c.Name == companyName01);
            return new List<ApplicationUser>()
            {
                new ApplicationUser { UserName = "NguyenVanHien", Email = "NguyenVanHien@yahoo.com", SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = "Hiền", LastName = "Nguyễn Văn", EmailConfirmed = true, Address = "73 Tô Thất Tùng, Huế",
                    Role = RoleValues.RepairPerson.ToDescription(), IsEnabled = true, Note = "Repair Person", Gender = Gender.Female.ToDescription(),
                    DateOfBirth = DateTime.ParseExact("20/10/1995", "dd/MM/yyyy", ci), PhoneNumber =  "0965847596",
                    AdminId = admin.Id, Company = company  },
                new ApplicationUser { UserName = "NguyenVanHau", Email = "NguyenVanHau@yahoo.com", SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = "Hậu", LastName = "Nguyễn Văn", EmailConfirmed = true, Address = "43 Tô Thất Tùng, Huế",
                    Role = RoleValues.RepairPerson.ToDescription(), IsEnabled = true, Note = "Repair Person", Gender = Gender.Male.ToDescription(),
                    DateOfBirth = DateTime.ParseExact("20/10/1994", "dd/MM/yyyy", ci), PhoneNumber =  "0968572435",
                    AdminId = admin.Id, Company = company  }
            };
        }

        public static async Task<IEnumerable<Request>> RequestDataAsync(TmanagerServiceContext dbContext, UserManager<ApplicationUser> userManager)
        {
            ApplicationUser admin = await userManager.FindByNameAsync(AdminData(dbContext).ToArray().FirstOrDefault().UserName);
            ApplicationUser supervisor = await userManager.FindByNameAsync((await SupervisorDataAsync(dbContext, userManager)).ToArray().FirstOrDefault().UserName);
            ApplicationUser repairPerson = await userManager.FindByNameAsync((await RepairPersonDataAsync(dbContext, userManager)).ToArray().FirstOrDefault().UserName);
            //AreaWorking areaWorking = dbContext.AreaWorkings.FirstOrDefault(a => a.AreaName == areaWorkingName01);
            //AreaWorking[] areaWorkingData = dbContext.AreaWorkings.ToArray();
            Company company = dbContext.Companys.FirstOrDefault(c => c.Name == companyName01);
            DateTime now = DateTime.Now;
            return new List<Request>()
            {
                new Request { Content = "ổ gà có trứng gà", PictureRequest = "http://res.cloudinary.com/dj9j7j4sf/image/upload/v1541144270/bbhvdqd1jz9xxrcvszc3.jpg,NStr~| http://res.cloudinary.com/dj9j7j4sf/image/upload/v1541144270/xpr0nra3lfk1fxtqhodz.jpg,NStr~| http://res.cloudinary.com/dj9j7j4sf/image/upload/v1541144271/dd8zsydl9hxhwt3qzcrl.jpg",
                PictureFinish = "http://res.cloudinary.com/dj9j7j4sf/image/upload/v1541144224/np7rbqxwektldyxw1kat.jpg,NStr~| http://res.cloudinary.com/dj9j7j4sf/image/upload/v1541144225/sdapicoqbredxjrfdf5u.jpg",
                Latlng_latitude = 16.047194, Latlng_longitude = 108.211754, Supervisor = supervisor, RepairPerson = repairPerson,
                Address="403 Đường Trưng Nữ Vương, Phường Hòa Thuận Tây, Quận Hải Châu, Thành Phố Đà Nẵng, Hòa Thuận Nam, Hải Châu, Đà Nẵng 550000, Việt Nam",
                Status = RequestStatus.Approved.ToDescription(), Company = company,
                TimeBeginRequest =  now.AddSeconds(-2563125), TimeReceiveRequest = now.AddSeconds(-2043125),
                TimeFinish = now.AddSeconds(-563425), TimeConfirm = now.AddSeconds(-253125) },
                new Request { Content = "ổ vịt, ổ trâu, ổ khủng long", PictureRequest = "http://res.cloudinary.com/dj9j7j4sf/image/upload/v1541144160/yvswxuhbcbbzrba2ti10.jpg,NStr~| http://res.cloudinary.com/dj9j7j4sf/image/upload/v1541144161/bfdm3bknugvdfoh5u6ec.jpg",
                PictureFinish = "http://res.cloudinary.com/dj9j7j4sf/image/upload/v1541144225/sdapicoqbredxjrfdf5u.jpg,NStr~| http://res.cloudinary.com/dj9j7j4sf/image/upload/v1541144226/zotgb0n59b3qgpq8xour.jpg,NStr~| http://res.cloudinary.com/dj9j7j4sf/image/upload/v1541144226/bkdzmmsu3gbeny0egdbf.jpg",
                Latlng_latitude = 16.041583, Latlng_longitude = 108.211631, Supervisor = supervisor, RepairPerson = repairPerson,
                Address="86-118 Lương Nhữ Hộc, Hoà Cường Bắc, Hải Châu, Đà Nẵng 550000, Việt Nam",
                Status = RequestStatus.Approved.ToDescription(), Company = company,
                TimeBeginRequest =  now.AddSeconds(-3563125), TimeReceiveRequest = now.AddSeconds(-3043125),
                TimeFinish = now.AddSeconds(-1563425), TimeConfirm = now.AddSeconds(-1253125) }
            };
        }

    }
}
