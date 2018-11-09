using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Threading.Tasks;
using TmanagerService.Core.Entities;

namespace TmanagerService.Api.Middleware
{
    public class CheckEnableUserMiddleware
    {
        private readonly RequestDelegate _next;
        public CheckEnableUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            ApplicationUser curUser = await userManager.GetUserAsync(context.User);
            var signInManager = context.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();
            if (curUser != null && !curUser.IsEnabled)
            {
                await signInManager.SignOutAsync();
                context.Response.StatusCode = (int) HttpStatusCode.Forbidden;
            }
            else
                await _next(context);
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
