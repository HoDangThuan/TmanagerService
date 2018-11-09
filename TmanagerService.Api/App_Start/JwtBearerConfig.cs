using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using TmanagerService.Api.Models;

namespace TmanagerService.Api.App_Start
{
    public class JwtBearerConfig
    {
        public static void Config(JwtBearerOptions options)
        {
            //options.RequireHttpsMetadata = false;
            //options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.FromMinutes(Convert.ToInt32(Startup.Configuration["Jwt:expiryMinutes"])),
                RequireExpirationTime = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Startup.Configuration["Jwt:issuer"],
                ValidAudience = Startup.Configuration["Jwt:issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Startup.Configuration["Jwt:secretKey"]))
            };
        }
    }
}
