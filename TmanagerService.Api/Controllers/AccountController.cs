using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using TmanagerService.Api.Identity;
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
        private readonly IEmailSender _emailSender;
        private readonly TmanagerServiceContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly HttpContext _httpContext;
        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            //HttpContext httpContext,
            TmanagerServiceContext context
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _emailSender = emailSender;
            //_httpContext = httpContext;
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
                return BadRequest(result.RequiresTwoFactor);
            }
            if (result.IsLockedOut)
            {
                return BadRequest(result.IsLockedOut);
            }

            return BadRequest();
        }

        [HttpPost("Register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] TmanagerRegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }

            if (await _userManager.FindByNameAsync(registerModel.UserName) != null)
            {
                return BadRequest("UserName already exists");
            }

            if (await _userManager.FindByEmailAsync(registerModel.Email) != null)
            {
                return BadRequest("Email already exists");
            }
            try
            {
                if (_context.Users.Select(u => u.PhoneNumber.Equals(registerModel.PhoneNumber)).FirstOrDefault())
                    return BadRequest("Phone number already exists");

                ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);
                var company = _context.Companys.FirstOrDefault(c => c.Id == registerModel.CompanyId);
                if (company == null || !company.Status)
                    return BadRequest("Company failed");
                string role;
                if (company.IsDepartmentOfConstruction)
                    role = RoleValues.Supervisor.ToDescription();
                else
                    role = RoleValues.RepairPerson.ToDescription();
                if (((Gender)registerModel.Gender).ToDescription() == null)
                    return BadRequest("Gender required");
                var user = new ApplicationUser
                {
                    UserName = registerModel.UserName,
                    Email = registerModel.Email,
                    PhoneNumber = registerModel.PhoneNumber,
                    DateOfBirth = null,
                    Company = company,
                    Role = role,
                    Gender = ((Gender)registerModel.Gender).ToDescription(),
                    AdminId = curUser.Id,
                    IsEnabled = true,
                    EmailConfirmed = true
                };
                string passwordStr = RdmStrExtension.RandomString(9);
                var result = await _userManager.CreateAsync(user, passwordStr);
                if (result.Succeeded)
                {
                    var userClaim = await _userManager.GetClaimsAsync(user);

                    if (user.Role == RoleValues.RepairPerson.ToDescription())
                    {
                        var repairPersonRoleString = RoleValues.RepairPerson.ToDescription();
                        var repairPersonClaimString = Claim.ReceiveRequest.ToDescription();

                        if (!await _userManager.IsInRoleAsync(user, repairPersonRoleString))
                        {
                            await _userManager.AddToRoleAsync(user, repairPersonRoleString);
                        }

                        if (!userClaim.Any(x => x.Type == repairPersonClaimString))
                        {
                            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim(repairPersonClaimString, true.ToString()));
                        }
                    }

                    if (user.Role == RoleValues.Supervisor.ToDescription())
                    {
                        var supervisorRoleString = RoleValues.Supervisor.ToDescription();
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

                    string subject = "Welcome to Tmanager";
                    string message = "Congratulations and thanks for signing up!"
                        + "<br/>Account Details: <br/> Email: " + registerModel.Email
                        + "<br/> User Name: " + registerModel.UserName
                        + "<br/> Default Password: " + passwordStr;
                    await _emailSender.SendEmailAsync(registerModel.Email, subject, message);

                    return Ok(true);
                }

                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel changePasswordModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }

            if (changePasswordModel.NewPassword != changePasswordModel.NewPasswordConfirm)
            {
                return BadRequest("Password confirm failed");
            }

            if (changePasswordModel.NewPassword == changePasswordModel.CurrentPassword)
            {
                return BadRequest("New password duplicate current password");
            }

            ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);

            var result = await _userManager.ChangePasswordAsync(curUser, changePasswordModel.CurrentPassword, changePasswordModel.NewPassword);
            if (result.Succeeded)
            {
                try
                {
                    DateTime now = DateTime.Now;
                    string subject = "Change your Tmanager account password";
                    string message = "Your account has changed <br/>Passwords for Tmanager account "
                        + curUser.UserName + " has been changed to "
                        + now.ToDateTimeFull();
                    await _emailSender.SendEmailAsync(curUser.Email, subject, message);
                    return Ok(true);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
            else
                return BadRequest(result.Errors);
        }

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel forgotPasswordModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }

            ApplicationUser user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);

            if (user == null)
                return BadRequest("Email does not exist");
            if (!(_userManager.IsEmailConfirmedAsync(user).Result))
                return BadRequest("Email does not active");
            if (user.UserName != forgotPasswordModel.UserName)
                return BadRequest("Account does not use this email");

            try
            {
                var tokenRSPW = await _userManager.GeneratePasswordResetTokenAsync(user);

                string passwordStr = RdmStrExtension.RandomString(9);
                var result = await _userManager.ResetPasswordAsync(user, tokenRSPW, passwordStr);
                if (result.Succeeded)
                {
                    string subject = "Forgot password Tmanager";
                    string message = "New password of your account " + user.UserName + " : " + passwordStr;
                    await _emailSender.SendEmailAsync(forgotPasswordModel.Email, subject, message);
                    return Ok(true);
                }
                else
                    return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("ChangeInformationUser")]
        [Authorize]
        public async Task<ActionResult> ChangeInformationUser([FromBody] ChangeInformationUserModel changeInformationUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }

            try
            {
                DateTime TimeBegin = Convert.ToDateTime("01-01-1900");
                DateTime TimeEnd = DateTime.Now.AddYears(-16);
                DateTime? dateOfBirth = null;
                if (changeInformationUserModel.DateOfBirth != null)
                {
                    dateOfBirth = changeInformationUserModel.DateOfBirth.ConvertDateTime();
                    if (dateOfBirth < TimeBegin || dateOfBirth > TimeEnd)
                        return BadRequest("Date of birth failed");
                }

                ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);
                if (changeInformationUserModel.Address != null)
                    curUser.Address = changeInformationUserModel.Address;
                if (dateOfBirth != null)
                    curUser.DateOfBirth = dateOfBirth;
                if (changeInformationUserModel.FirstName != null)
                    curUser.FirstName = changeInformationUserModel.FirstName;
                if (changeInformationUserModel.LastName != null)
                    curUser.LastName = changeInformationUserModel.LastName;
                if (changeInformationUserModel.PhoneNumber != null)
                    curUser.PhoneNumber = changeInformationUserModel.PhoneNumber;

                await _userManager.UpdateAsync(curUser);
                await _context.SaveChangesAsync();
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("GetNameUser")]
        [Authorize]
        public async Task<ActionResult> GetNameUser()
        {
            ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);
            return Ok(new
            {
                id = curUser.Id,
                userName = curUser.UserName,
                firstName = curUser.FirstName,
                lastName = curUser.LastName,
                position = curUser.Role,
                //area = String_List.ToList(curUser.ListAreaWorkingId)
            });
        }

        [HttpGet("GetUserInformation")]
        [Authorize]
        public async Task<ActionResult> GetUserInformation()
        {
            ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);

            if (!curUser.IsEnabled)
                return BadRequest("Account blocked");
            InformationUser infoUser = new InformationUser
            {
                Id = curUser.Id,
                UserName = curUser.UserName,
                FirstName = curUser.FirstName,
                LastName = curUser.LastName,
                Address = curUser.Address,
                Role = curUser.Role,
                PhoneNumber = curUser.PhoneNumber,
                Email = curUser.Email,
                //ListAreaWorking = GetListAreaWorkingInfo(curUser),
                DateOfBirth = curUser.DateOfBirth.ToDate(),
                Gender = curUser.Gender,
                IsEnabled = curUser.IsEnabled,
                Note = curUser.Note
            };
            return Ok(infoUser);
        }

        [HttpGet("GetPositionByAdmin")]
        [Authorize(Roles = "Admin")]
        public ActionResult GetPositionByAdmin()
        {
            var roleValues = Enum.GetValues(typeof(RoleValues));
            List<string> result = new List<string>();
            foreach (var role in roleValues)
                if (role.ToString() != RoleValues.Admin.ToDescription())
                    result.Add(role.ToString());
            return Ok(result);
        }

        [HttpGet("GetStaffByAdmin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetStaffByAdmin()
        {
            ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);
            List<ApplicationUser> lstStf = (from us in _context.Users
                                            where us.AdminId == curUser.Id
                                            select us).ToList();
            List<InformationUser> listStaff = new List<InformationUser>();
            foreach (var staff in lstStf)
            {
                if (staff != null)
                    listStaff.Add(new InformationUser
                    {
                        Id = staff.Id,
                        UserName = staff.UserName,
                        FirstName = staff.FirstName,
                        LastName = staff.LastName,
                        Address = staff.Address,
                        Role = staff.Role,
                        PhoneNumber = staff.PhoneNumber,
                        Email = staff.Email,
                        //ListAreaWorking = GetListAreaWorkingInfo(staff),
                        DateOfBirth = staff.DateOfBirth.ToDate(),
                        Gender = staff.Gender,
                        Note = staff.Note,
                        IsEnabled = staff.IsEnabled
                    });
            }
            return Ok(listStaff);
        }

        [HttpGet("GetStaffInfoById")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetStaffByAdmin(string staffId)
        {
            ApplicationUser staff = await _userManager.FindByIdAsync(staffId);
            //List<AreaWorkingValues> listAreaWorking = new List<AreaWorkingValues>();
            //AreaWorkingValues areaWorking = new AreaWorkingValues();
            //foreach (var areaWorkingId in String_List.ToList(staff.ListAreaWorkingId))
            //{
            //    areaWorking = (from arW in _context.AreaWorkings
            //                   where arW.Id == areaWorkingId
            //                   select new AreaWorkingValues
            //                   {
            //                       AreaId = arW.Id,
            //                       AdminId = arW.AdminId,
            //                       AreaName = arW.AreaName,
            //                       Status = arW.Status
            //                   }).FirstOrDefault();
            //    if (areaWorking != null)
            //        listAreaWorking.Add(areaWorking);
            //}
            return Ok(new InformationUser
            {
                Id = staff.Id,
                UserName = staff.UserName,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                Address = staff.Address,
                Role = staff.Role,
                PhoneNumber = staff.PhoneNumber,
                Email = staff.Email,
                //ListAreaWorking = GetListAreaWorkingInfo(staff),
                DateOfBirth = staff.DateOfBirth.ToDate(),
                Gender = staff.Gender,
                Note = staff.Note,
                IsEnabled = staff.IsEnabled
            });
        }

        [HttpGet("DisableAccount")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DisableAccount(DisableAccountModel disableAccountModel)
        {
            ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);
            List<ApplicationUser> lstStf = (from us in _context.Users
                                            where us.AdminId == curUser.Id
                                            select us).ToList();
            List<InformationUser> listStaff = new List<InformationUser>();
            foreach (var staff in lstStf)
            {
                if (staff != null)
                    listStaff.Add(new InformationUser
                    {
                        Id = staff.Id,
                        UserName = staff.UserName,
                        FirstName = staff.FirstName,
                        LastName = staff.LastName,
                        Address = staff.Address,
                        Role = staff.Role,
                        PhoneNumber = staff.PhoneNumber,
                        Email = staff.Email,
                        //ListAreaWorking = GetListAreaWorkingInfo(staff),
                        DateOfBirth = staff.DateOfBirth.ToDate(),
                        Gender = staff.Gender,
                        Note = staff.Note,
                        IsEnabled = staff.IsEnabled
                    });
            }
            return Ok(listStaff);
        }

        //private List<AreaWorkingValues> GetListAreaWorkingInfo(ApplicationUser user)
        //{
        //    if (user.ListAreaWorkingId == null)
        //        return null;
        //    List<AreaWorkingValues> listAreaWorking = new List<AreaWorkingValues>();
        //    AreaWorkingValues areaWorking = new AreaWorkingValues();
        //    string adminRoleStr = RoleValues.Admin.ToDescription();
        //    foreach (var areaWorkingId in String_List.ToList(user.ListAreaWorkingId))
        //    {
        //        areaWorking = (from arW in _context.AreaWorkings
        //                       where arW.Id == areaWorkingId &&
        //                            (
        //                                (arW.Status && user.Role != adminRoleStr) ||
        //                                (user.Role == adminRoleStr)
        //                            )
        //                       select new AreaWorkingValues
        //                       {
        //                           AreaId = arW.Id,
        //                           AdminId = arW.AdminId,
        //                           AreaName = arW.AreaName,
        //                           Status = arW.Status
        //                       }).FirstOrDefault();
        //        if (areaWorking != null)
        //            listAreaWorking.Add(areaWorking);
        //    }
        //    if (listAreaWorking.Count > 0)
        //        return listAreaWorking;
        //    return null;
        //}

    }
}