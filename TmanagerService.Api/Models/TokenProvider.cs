using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TmanagerService.Core.Entities;

namespace TmanagerService.Api.Models
{
    public class TokenProvider
    {
        public static async Task<string> ExecuteAsync(ApplicationUser user, UserManager<ApplicationUser> _userManager)
        {
            
            var now = DateTime.UtcNow;
            var expTime = now.AddDays(Convert.ToInt32(Startup.Configuration["Jwt:refreshTokenExpireDays"]));

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now)
                    .ToUniversalTime().ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(expTime)
                    .ToUniversalTime().ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            foreach (var claim in userClaims)
            {
                claims.Add(new Claim(claim.Type, claim.Value));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Startup.Configuration["Jwt:secretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(Startup.Configuration["Jwt:issuer"],
                Startup.Configuration["Jwt:issuer"],
                claims,
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /*
         public static TokenProviderOptions GetOptions()
         {
             var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Startup.Configuration.GetSection("Jwt_key").Value));

             return new TokenProviderOptions
             {
                 Path = Startup.Configuration.GetSection("Jwt_token_path").Value,
                 Audience = Startup.Configuration.GetSection("Jwt_audience").Value,
                 Issuer = Startup.Configuration.GetSection("Jwt_issuer").Value,
                 Expiration = TimeSpan.FromMinutes(Convert.ToInt32(Startup.Configuration.GetSection("Jwt_expire_minutes").Value)),
                 RefreshTokenExpiration = TimeSpan.FromDays(Convert.ToInt32(Startup.Configuration.GetSection("Jwt_refresh_token_expire_days").Value)),
                 SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
             };
         }

         public static async Task<LoginResponseData> ExecuteAsync(ApplicationUser user, TmanagerServiceContext db, 
             UserManager<ApplicationUser> _userManager, RefreshToken refreshToken = null)
         {
             var options = GetOptions();
             var now = DateTime.UtcNow;

             var claims = new List<Claim>()
             {
                 new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                 new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
                 new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                 new Claim(JwtRegisteredClaimNames.Gender, user.Gender),
                 new Claim(JwtRegisteredClaimNames.Birthdate, user.DateOfBirth.ToString("dd-MM-yyyy")),
                 new Claim(JwtRegisteredClaimNames.Email, user.Email),
                 new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUniversalTime().ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                 new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
             };

             var userRoles = await _userManager.GetRolesAsync(user);
             foreach (var role in userRoles)
             {
                 claims.Add(new Claim(ClaimTypes.Role, role));
             }

             var userClaims = await _userManager.GetClaimsAsync(user);
             foreach (var claim in userClaims)
             {
                 claims.Add(new Claim(claim.Type, claim.Value));
             }

             if (refreshToken == null)
             {
                 refreshToken = new RefreshToken()
                 {
                     UserId = user.Id,
                     Token = Guid.NewGuid().ToString("N"),
                 };
                 refreshToken.IssuedUtc = now;
                 refreshToken.ExpiresUtc = now.Add(options.RefreshTokenExpiration);
                 db.InsertNewRefreshToken(refreshToken);
             }
             else
             {
                 refreshToken.IssuedUtc = now;
                 refreshToken.ExpiresUtc = now.Add(options.RefreshTokenExpiration);
                 db.SaveChanges();
             }

             var jwt = new JwtSecurityToken(
                 issuer: options.Issuer,
                 audience: options.Audience,
                 claims: claims.ToArray(),
                 notBefore: now,
                 expires: now.Add(options.Expiration),
                 signingCredentials: options.SigningCredentials);
             var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

             var response = new LoginResponseData
             {
                 AccessToken = encodedJwt,
                 RefreshToken = refreshToken.Token,
                 ExpriresIn = (int)options.Expiration.TotalSeconds,
                 UserName = user.UserName,
                 FirstName = user.FirstName,
                 LastName = user.LastName,
                 Roles = claims.Where(x => x.Type == ClaimInformation.RoleClaimType).Select(x => x.Value).ToList()
             };
             return response;
         }
         */
    }

    public struct ClaimInformation
    {
        public const string RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
    }
}
