using Microsoft.AspNetCore.Identity;

namespace TmanagerService.Api.App_Start
{
    public class IdentityConfig
    {
        public static void Config(IdentityOptions options)
        {
            options.SignIn.RequireConfirmedEmail = true;
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
        }
    }
}
