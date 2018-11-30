using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TmanagerService.Core.Entities;
using TmanagerService.Infrastructure.Data;

namespace TmanagerService.Api.Middleware
{
    public class CheckEnableUserMiddleware
    {
        private readonly RequestDelegate _next;
        public CheckEnableUserMiddleware(
            RequestDelegate next
            )
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var signInManager = context.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();
            
            ApplicationUser curUser = await userManager.GetUserAsync(context.User);

            if(curUser == null)
                await _next(context);
            else
            {
                var dbContext = context.RequestServices.GetService<TmanagerServiceContext>();
                Company company = dbContext.Companys.FirstOrDefault(c => c.Id == curUser.CompanyId);
                if (!curUser.IsEnabled || (company != null && !company.Status))
                {
                    await signInManager.SignOutAsync();
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                }
                await _next(context);
            }
        }
    }

    //public static class CheckEnableMiddlewareExtensions
    //{
    //    public static IApplicationBuilder UseCheckEnableUser(this IApplicationBuilder builder)
    //    {
    //        return builder.UseMiddleware<CheckEnableUserMiddleware>();
    //    }
    //}
}
