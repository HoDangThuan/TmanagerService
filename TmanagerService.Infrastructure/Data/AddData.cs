using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TmanagerService.Core.Entities;
using TmanagerService.Core.Enums;
using TmanagerService.Core.Extensions;

namespace TmanagerService.Infrastructure.Data
{
    public class AddData
    {
        public static async Task SeedRepairPerson(UserManager<ApplicationUser> userManager)
        {
            var repairPersonRoleString = Role.RepairPerson.ToDescription();
            var repairPersonClaimString = Claim.ReceiveRequest.ToDescription();

            foreach (var user in DataValues.RepairPersonData())
            {
                if (await userManager.FindByNameAsync(user.UserName) == null)
                {
                    await userManager.CreateAsync(user, DataValues.default_user_password);

                    var userClaim = await userManager.GetClaimsAsync(user);

                    if (!await userManager.IsInRoleAsync(user, repairPersonRoleString))
                    {
                        await userManager.AddToRoleAsync(user, repairPersonRoleString);
                    }

                    if (!userClaim.Any(x => x.Type == repairPersonClaimString))
                    {
                        await userManager.AddClaimAsync(user, new System.Security.Claims.Claim(repairPersonClaimString, true.ToString()));
                    }
                }
            }
        }

        public static async Task SeedSupervisor(UserManager<ApplicationUser> userManager)
        {
            var supervisorRoleString = Role.Supervisor.ToDescription();
            var supervisorClaimString = Claim.InsertRequest.ToDescription();

            foreach (var user in DataValues.SupervisorData())
            {
                if (await userManager.FindByNameAsync(user.UserName) == null)
                {
                    await userManager.CreateAsync(user, DataValues.default_user_password);

                    var userClaim = await userManager.GetClaimsAsync(user);

                    if (!await userManager.IsInRoleAsync(user, supervisorRoleString))
                    {
                        await userManager.AddToRoleAsync(user, supervisorRoleString);
                    }

                    if (!userClaim.Any(x => x.Type == supervisorClaimString))
                    {
                        await userManager.AddClaimAsync(user, new System.Security.Claims.Claim(supervisorClaimString, true.ToString()));
                    }
                }
            }
        }

        public static async Task SeedAdmin(UserManager<ApplicationUser> userManager)
        {
            var adminRoleString = Role.Admin.ToDescription();
            var adminClaimString = Claim.CreateAccount.ToDescription();

            foreach (var admin in DataValues.AdminData())
            {
                if (await userManager.FindByNameAsync(admin.UserName) == null)
                {
                    await userManager.CreateAsync(admin, DataValues.default_admin_password);

                    var adminClaim = await userManager.GetClaimsAsync(admin);

                    if (!await userManager.IsInRoleAsync(admin, adminRoleString))
                    {
                        await userManager.AddToRoleAsync(admin, adminRoleString);
                    }

                    if (!adminClaim.Any(x => x.Type == adminClaimString))
                    {
                        await userManager.AddClaimAsync(admin, new System.Security.Claims.Claim(adminClaimString, true.ToString()));
                    }
                }
            }
        }

        public static async Task SeedRolesAndClaims(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var adminRoleString = Role.Admin.ToDescription();
            var supervisorRoleString = Role.Supervisor.ToDescription();
            var repairPersonRoleString = Role.RepairPerson.ToDescription();

            if (!await roleManager.RoleExistsAsync(adminRoleString))
            {
                await roleManager.CreateAsync(new IdentityRole
                {
                    Name = adminRoleString
                });
            }

            if (!await roleManager.RoleExistsAsync(supervisorRoleString))
            {
                await roleManager.CreateAsync(new IdentityRole
                {
                    Name = supervisorRoleString
                });
            }

            if (!await roleManager.RoleExistsAsync(repairPersonRoleString))
            {
                await roleManager.CreateAsync(new IdentityRole
                {
                    Name = repairPersonRoleString
                });
            }

            var createAccountClaimString = Claim.CreateAccount.ToDescription();
            var insertRequestClaimString = Claim.InsertRequest.ToDescription();
            var receiveRequestClaimString = Claim.ReceiveRequest.ToDescription();

            var adminRole = await roleManager.FindByNameAsync(adminRoleString);
            var adminRoleClaims = await roleManager.GetClaimsAsync(adminRole);

            if (!adminRoleClaims.Any(x => x.Type == createAccountClaimString))
            {
                await roleManager.AddClaimAsync(adminRole, new System.Security.Claims.Claim(createAccountClaimString, "true"));
            }

            var supervisorRole = await roleManager.FindByNameAsync(supervisorRoleString);
            var supervisorRoleClaims = await roleManager.GetClaimsAsync(supervisorRole);
            if (!supervisorRoleClaims.Any(x => x.Type == insertRequestClaimString))
            {
                await roleManager.AddClaimAsync(supervisorRole, new System.Security.Claims.Claim(insertRequestClaimString, "true"));
            }

            var repairPersonRole = await roleManager.FindByNameAsync(repairPersonRoleString);
            var repairPersonRoleClaims = await roleManager.GetClaimsAsync(repairPersonRole);
            if (!repairPersonRoleClaims.Any(x => x.Type == receiveRequestClaimString))
            {
                await roleManager.AddClaimAsync(repairPersonRole, new System.Security.Claims.Claim(receiveRequestClaimString, "true"));
            }
        }
    }
}
