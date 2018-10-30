//using System;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using Microsoft.IdentityModel.Tokens;
//using TmanagerService.Core.Entities;
//using TmanagerService.Infrastructure.Data;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Identity;
//using System.ComponentModel.DataAnnotations;
//using Microsoft.Extensions.Logging;
//using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
//using System.Linq;

//namespace TmanagerService.Api.Controllers
//{
//    [Route("api/[controller]")]
//    public class AuthenticationController : ControllerBase
//    {
//        private readonly IConfiguration _config;
//        private readonly TmanagerServiceContext _context;
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly SignInManager<ApplicationUser> _signInManager;
//        private readonly ILogger<LoginModel> _logger;

//        public AuthenticationController(IConfiguration config,
//            UserManager<ApplicationUser> userManager,
//            SignInManager<ApplicationUser> signInManager,
//            ILogger<LoginModel> logger,
//            TmanagerServiceContext context)
//        {
//            _config = config;
//            _context = context;
//            _userManager = userManager;
//            _signInManager = signInManager;
//            _logger = logger;
//        }

//        [HttpPost]
//        public async Task<IActionResult> CreateToken([FromForm]InputModel input)
//        {
//            IActionResult response = Unauthorized();
//            ApplicationUser user = await Authenticate(input);

//            if (user != null & user.IsEnabled == true)
//            {
//                var tokenString = await BuildToken(user);
//                response = Ok(new { token = tokenString });
//            }

//            return response;
//        }

//        private async Task<string> BuildToken(ApplicationUser user)
//        {
//            var claims = new[] {
//                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
//                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
//                new Claim(JwtRegisteredClaimNames.Gender, user.Gender),
//                new Claim(JwtRegisteredClaimNames.Birthdate, user.DateOfBirth.ToString("dd-MM-yyyy")),
//                new Claim(JwtRegisteredClaimNames.Email, user.Email),
//                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
//            };

//            var roles = await _userManager.GetRolesAsync(user);
//            var lstClaims = claims.ToList();
//            foreach (var role in roles)
//            {
//                lstClaims.Add(new Claim(ClaimTypes.Role, role));
//            }
            
//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt_Key"]));
//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            var token = new JwtSecurityToken(_config["Jwt_Issuer"],
//                _config["Jwt_Issuer"],
//                lstClaims,
//                expires: DateTime.Now.AddMinutes(30), //expire time is 30 minutes
//                signingCredentials: creds);

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }

//        private async Task<ApplicationUser> Authenticate(InputModel input)
//        {
//            var result = await _signInManager.PasswordSignInAsync(input.UserName, input.Password, input.RememberMe, lockoutOnFailure: true);
//            if (result.Succeeded)
//            {                
//                return await _userManager.FindByNameAsync(input.UserName);
//            }
//            if (result.RequiresTwoFactor)
//            {
//                return new ApplicationUser { UserName = "Requires Two Factor", IsEnabled=true };
//            }
//            if (result.IsLockedOut)
//            {
//                return new ApplicationUser { UserName = "Account locked", IsEnabled = true };
//            }
//            else
//            {
//                return new ApplicationUser { UserName = "Login failed", IsEnabled = true };
//            }
//        }

//        public class InputModel
//        {
//            [Required]
//            public string UserName { get; set; }

//            [Required]
//            [DataType(DataType.Password)]
//            public string Password { get; set; }

//            [Display(Name = "Remember me?")]
//            public bool RememberMe { get; set; }
//        }
//    }
//}