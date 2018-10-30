using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TmanagerService.Core.Entities;
using TmanagerService.Core.Enums;
using TmanagerService.Core.Extensions;

namespace TmanagerService.Infrastructure.Data
{
    public class TmanagerServiceContextSeed
    {
        public static async Task SeedAsync(TmanagerServiceContext dbContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILoggerFactory loggerFactory, int? retry = 0)
        {
            int retryForAvailability = retry.Value;
            try
            {
                dbContext.Database.Migrate();
                await AddData.SeedRolesAndClaims(userManager, roleManager);
                
                //Seed Admin
                await AddData.SeedAdmin(dbContext, userManager);
                
                //Seed AreaWorking
                //if (!dbContext.AreaWorkings.Any())
                //{
                //    dbContext.AreaWorkings.AddRange(await DataValues.AreaWorkingDataAsync(dbContext, userManager));

                //    await dbContext.SaveChangesAsync();
                //}

                //Seed Admin
                if (!dbContext.Companys.Any())
                {
                    dbContext.Companys.AddRange(await DataValues.CompanyDataAsync(dbContext, userManager));

                    await dbContext.SaveChangesAsync();
                }

                await AddData.SeedSupervisor(dbContext, userManager);
                await AddData.SeedRepairPerson(dbContext, userManager);

                ////add listAreaWorking for Admin01
                //ApplicationUser admin = await userManager.FindByNameAsync(DataValues.adminName01);
                //List<string> listAreaWorking = (from arW in dbContext.AreaWorkings
                //                                where arW.AdminId == admin.Id
                //                                select arW.Id).ToList();
                //admin.ListAreaWorkingId = String_List.ToString(listAreaWorking);
                //await userManager.UpdateAsync(admin);

                //await dbContext.SaveChangesAsync();

                //Seed Request
                if (!dbContext.Requests.Any())
                {
                    dbContext.Requests.AddRange(await DataValues.RequestDataAsync(dbContext, userManager));

                    await dbContext.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                if (retryForAvailability < 10)
                {
                    retryForAvailability++;
                    var log = loggerFactory.CreateLogger<TmanagerServiceContext>();
                    log.LogError(ex.Message);
                    await SeedAsync(dbContext, userManager, roleManager, loggerFactory, retryForAvailability);
                }
            }
        }      

    }
}