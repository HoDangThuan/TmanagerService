using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TmanagerService.Core.Entities;

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

                await AddData.SeedAdmin(userManager);
                await AddData.SeedSupervisor(userManager);
                await AddData.SeedRepairPerson(userManager);
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