using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

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
                ClockSkew = TimeSpan.FromMinutes(Convert.ToInt32(Startup.Configuration["Jwt_expire_minutes"])),
                RequireExpirationTime = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Startup.Configuration["Jwt_Issuer"],
                ValidAudience = Startup.Configuration["Jwt_Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Startup.Configuration["Jwt_Key"]))
            };
        }
    }
}
