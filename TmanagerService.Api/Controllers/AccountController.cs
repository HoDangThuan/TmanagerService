using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TmanagerService.Api.InpuModels;
using TmanagerService.Api.InputModels;
using TmanagerService.Api.Models;
using TmanagerService.Core.Entities;
using TmanagerService.Core.Enums;
using TmanagerService.Core.Extensions;
using TmanagerService.Infrastructure.Data;

namespace TmanagerService.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : ControllerBase
    {
        private readonly TmanagerServiceContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            TmanagerServiceContext context
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        [HttpPut("ChangeInformationUser")]
        [Authorize]
        public async Task<ActionResult> ChangeInformationUser([FromBody] ChangeInformationUserModel changeInformationUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }

            ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);
            try
            {
                if (changeInformationUserModel.Address != null)
                    curUser.Address = changeInformationUserModel.Address;
                if (changeInformationUserModel.DateOfBirth != null)
                    curUser.DateOfBirth = changeInformationUserModel.DateOfBirth;
                if (changeInformationUserModel.Email != null)
                    curUser.Email = changeInformationUserModel.Email;
                if (changeInformationUserModel.FirstName != null)
                    curUser.FirstName = changeInformationUserModel.FirstName;
                if (changeInformationUserModel.LastName != null)
                    curUser.LastName = changeInformationUserModel.LastName;
                if (changeInformationUserModel.PhoneNumber != null)
                    curUser.PhoneNumber = changeInformationUserModel.PhoneNumber;
                if (changeInformationUserModel.Gender != null)
                    curUser.Gender = changeInformationUserModel.Gender;

                await _userManager.UpdateAsync(curUser);
                await _context.SaveChangesAsync();
                return Ok(true);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Join(Environment.NewLine, ex));
            }
        }

        [HttpPut("ChangeAreaUser")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ChangeAreaUser([FromBody] ChangeAreaUserModel changeAreaUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }
            ApplicationUser user = await _userManager.FindByNameAsync(changeAreaUserModel.UserName);
            if(user != null)
            {
                try
                {
                    user.Area = changeAreaUserModel.Area;
                    await _userManager.UpdateAsync(user);
                    await _context.SaveChangesAsync();
                    return Ok(true);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(string.Join(Environment.NewLine, ex));
                }
            }

            throw new ApplicationException(string.Join(Environment.NewLine, changeAreaUserModel.UserName + " does not exist"));
        }

        [HttpGet("GetNameUser")]
        [Authorize]
        public async Task<ActionResult> GetNameUser()
        {
            ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);

            return Ok(new
            {
                userName = curUser.UserName,
                firstName = curUser.FirstName,
                lastName = curUser.LastName,
                position = curUser.Position,
                area = curUser.Area
            });
        }

        [HttpGet("GetUserInformation")]
        [Authorize]
        public async Task<ActionResult> GetUserInformation()
        {
            ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);

            return Ok(new
            {
                userName = curUser.UserName,
                firstName = curUser.FirstName,
                lastName = curUser.LastName,
                address = curUser.Address,
                position = curUser.Position,
                phoneNumber = curUser.PhoneNumber,
                email = curUser.Email,
                are = curUser.Area,
                dateOfBirth = curUser.DateOfBirth,
                gender = curUser.Gender,
                note = curUser.Note,
                isEnabled = curUser.IsEnabled
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] TmanagerLoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }

            var result = await _signInManager.PasswordSignInAsync(loginModel.UserName, loginModel.Password, loginModel.RememberMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(loginModel.UserName);
                string tokenResponse = await TokenProvider.ExecuteAsync(user, _context, _userManager);

                //Refresh Token
                user.Token = tokenResponse;
                await _userManager.UpdateAsync(user);

                return Ok(new { token = tokenResponse });
            }
            if (result.RequiresTwoFactor)
            {
                throw new ApplicationException(string.Join(Environment.NewLine, "Requires Two Factor"));
            }
            if (result.IsLockedOut)
            {
                throw new ApplicationException(string.Join(Environment.NewLine, "Account Is Locked Out"));
            }

            throw new ApplicationException(string.Join(Environment.NewLine, "Password Failed"));
        }

        [HttpPost("Register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] TmanagerRegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }

            if(registerModel.Password != registerModel.PasswordConfirm)
            {
                throw new ApplicationException(string.Join(Environment.NewLine, "Password Confirm Failed"));
            }
            var usr = await _userManager.FindByNameAsync(registerModel.UserName);
            if (usr != null)
            {
                throw new ApplicationException(string.Join(Environment.NewLine, "UserName already exists"));
            }

            var user = new ApplicationUser
            {
                UserName = registerModel.UserName,
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                Email = registerModel.Email,
                IsEnabled = true,
                DateOfBirth = registerModel.DateOfBirth,
                Gender = registerModel.Gender,
                PhoneNumber = registerModel.PhoneNumber,
                Address = registerModel.Address,
                Position = registerModel.Position,
                Area = registerModel.Area,
                Note = registerModel.Note,
                //Confirm Email, PhoneNumber để bổ sung sau
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);
            if (result.Succeeded)
            {
                var userClaim = await _userManager.GetClaimsAsync(user);

                if (registerModel.Position==Role.RepairPerson.ToDescription())
                {
                    var repairPersonRoleString = Role.RepairPerson.ToDescription();
                    var repairPersonClaimString = Claim.ReceiveRequest.ToDescription();

                    await _userManager.CreateAsync(user, DataValues.default_user_password);

                    if (!await _userManager.IsInRoleAsync(user, repairPersonRoleString))
                    {
                        await _userManager.AddToRoleAsync(user, repairPersonRoleString);
                    }

                    if (!userClaim.Any(x => x.Type == repairPersonClaimString))
                    {
                        await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim(repairPersonClaimString, true.ToString()));
                    }
                }
                else
                {
                    var supervisorRoleString = Role.Supervisor.ToDescription();
                    var supervisorClaimString = Claim.InsertRequest.ToDescription();

                    if (!await _userManager.IsInRoleAsync(user, supervisorRoleString))
                    {
                        await _userManager.AddToRoleAsync(user, supervisorRoleString);
                    }

                    if (!userClaim.Any(x => x.Type == supervisorClaimString))
                    {
                        await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim(supervisorClaimString, true.ToString()));
                    }
                }

                string tokenResponse = await TokenProvider.ExecuteAsync(user, _context, _userManager);
                
                return Ok(new { token = tokenResponse });
            }

            throw new ApplicationException(string.Join(Environment.NewLine, result.Errors));
        }
               
        //[HttpPost("RegisterRepairPerson")]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> RegisterRepairPerson(RegisterModel input)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState.GetErrors());
        //    }
        //    if (input.Password != input.PasswordConfirm)
        //    {
        //        throw new ApplicationException(string.Join(Environment.NewLine, "Password Confirm Failed"));
        //    }
        //    var user = new ApplicationUser
        //    {
        //        UserName = input.Email,
        //        FirstName = input.FirstName,
        //        LastName = input.LastName,
        //        Email = input.Email,
        //        IsEnabled = true,
        //        DateOfBirth = input.DateOfBirth,
        //        Gender = input.Gender,
        //        PhoneNumber = input.PhoneNumber,
        //        Address = input.Address,
        //        Position = input.Position,
        //        Note = input.Note,
        //        //Confirm Email, PhoneNumber để bổ sung sau
        //        EmailConfirmed = true
        //    };

        //    var result = await _userManager.CreateAsync(user, input.Password);
        //    if (result.Succeeded)
        //    {
        //        var repairPersonRoleString = Role.RepairPerson.ToDescription();
        //        var repairPersonClaimString = Claim.ReceiveRequest.ToDescription();
        //        var userClaim = await _userManager.GetClaimsAsync(user);

        //        await _userManager.CreateAsync(user, DataValues.default_user_password);
                    
        //        if (!await _userManager.IsInRoleAsync(user, repairPersonRoleString))
        //        {
        //            await _userManager.AddToRoleAsync(user, repairPersonRoleString);
        //        }

        //        if (!userClaim.Any(x => x.Type == repairPersonClaimString))
        //        {
        //            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim(repairPersonClaimString, true.ToString()));
        //        }

        //        return Ok(TokenProvider.ExecuteAsync(user, _db, _userManager));
        //    }

        //    throw new ApplicationException(string.Join(Environment.NewLine, result.Errors));
        //}

        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] TmanagerResetPasswordModel resetPasswordModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }

            //var user = await _userManager.FindByNameAsync(curUser.UserName);
            //if (user == null || !(_userManager.IsEmailConfirmedAsync(user).Result))
            //{
            //    return BadRequest();
            //}

            if(resetPasswordModel.NewPassword != resetPasswordModel.NewPasswordConfirm)
            {
                throw new ApplicationException(string.Join(Environment.NewLine, "Password confirm failed"));
            }

            if (resetPasswordModel.NewPassword == resetPasswordModel.CurrentPassword)
            {
                throw new ApplicationException(string.Join(Environment.NewLine, "New password duplicate current password"));
            }

            ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);

            var result = await _userManager.ChangePasswordAsync(curUser, resetPasswordModel.CurrentPassword, resetPasswordModel.NewPassword);
            if(result.Succeeded)
            {
                return Ok(true);
            }
            else
                throw new ApplicationException(string.Join(Environment.NewLine, result.Errors));
        }
    }
}