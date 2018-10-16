using Microsoft.IdentityModel.Tokens;
using System;

namespace TmanagerService.Api.Models
{
    public class TokenProviderOptions
    {
        public string Path { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public TimeSpan Expiration { get; set; }
        public TimeSpan RefreshTokenExpiration { get; set; }
        public SigningCredentials SigningCredentials { get; set; }
    }
}
