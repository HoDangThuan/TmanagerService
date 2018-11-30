using System;
using System.Collections.Generic;
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
        private readonly ITokenManager _tokenManager;
        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            ITokenManager tokenManager,
            TmanagerServiceContext context
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _emailSender = emailSender;
            _tokenManager = tokenManager;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] TmanagerLoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }

            var user = await _userManager.FindByNameAsync(loginModel.UserName);
            var company_user = _context.Companys.FirstOrDefault(c => c.Id == user.CompanyId);

            if (!user.IsEnabled)
                return BadRequest("Account is blocked");
            if (user.Role != RoleValues.Admin.ToDescription() && 
                company_user != null && !company_user.Status)
                return BadRequest("Your company is blocked");

            var result = await _signInManager.PasswordSignInAsync(loginModel.UserName, loginModel.Password, loginModel.RememberMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                string tokenResponse = await TokenProvider.ExecuteAsync(user, _userManager);
                
                return Ok(new { token = tokenResponse });
            }
            if (result.RequiresTwoFactor)
            {
                return BadRequest("Requires Two Factor");
            }
            if (result.IsLockedOut)
            {
                return BadRequest("Is Locked Out");
            }

            return BadRequest();
        }

        [HttpPut("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                //revoke token
                await _tokenManager.DeactivateCurrentAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                    //revoke token
                    await _tokenManager.DeactivateCurrentAsync();

                    //send email
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
                ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);


                if (_context.Users.Select(u => u.PhoneNumber.Equals(changeInformationUserModel.PhoneNumber)).FirstOrDefault())
                    return BadRequest("Phone number already exists");
                if (changeInformationUserModel.PhoneNumber != null)
                    curUser.PhoneNumber = changeInformationUserModel.PhoneNumber;

                DateTime TimeBegin = Convert.ToDateTime("01-01-1900");
                DateTime TimeEnd = DateTime.Now.AddYears(-16);
                DateTime? dateOfBirth = null;
                if (changeInformationUserModel.DateOfBirth != null)
                {
                    dateOfBirth = changeInformationUserModel.DateOfBirth.ConvertDateTime();
                    if (dateOfBirth < TimeBegin || dateOfBirth > TimeEnd)
                        return BadRequest("Date of birth failed");
                }
                if (changeInformationUserModel.Address != null)
                    curUser.Address = changeInformationUserModel.Address;
                if (dateOfBirth != null)
                    curUser.DateOfBirth = dateOfBirth;
                if (changeInformationUserModel.FirstName != null)
                    curUser.FirstName = changeInformationUserModel.FirstName;
                if (changeInformationUserModel.LastName != null)
                    curUser.LastName = changeInformationUserModel.LastName;

                var result = await _userManager.UpdateAsync(curUser);
                if (result.Succeeded)
                    return Ok(true);
                else
                    return BadRequest(result.Errors);
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
                Avatar = curUser.Avatar,
                CompanyId = curUser.CompanyId,
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
            ApplicationUser admin = await _userManager.GetUserAsync(HttpContext.User);
            List<InformationUser> lstStf = (from staff in _context.Users
                                            where staff.AdminId == admin.Id
                                            select new InformationUser
                                            {
                                                Id = staff.Id,
                                                UserName = staff.UserName,
                                                FirstName = staff.FirstName,
                                                LastName = staff.LastName,
                                                Address = staff.Address,
                                                Role = staff.Role,
                                                PhoneNumber = staff.PhoneNumber,
                                                Email = staff.Email,
                                                DateOfBirth = staff.DateOfBirth.ToDate(),
                                                Gender = staff.Gender,
                                                Avatar = staff.Avatar,
                                                CompanyId = staff.CompanyId,
                                                Note = staff.Note,
                                                IsEnabled = staff.IsEnabled
                                            }).ToList();
            return Ok(lstStf);
            //int lstSize = lstStf.Count(),
            //    start = (pageIndex - 1) * pageSize;
            //if (start + pageSize > lstSize)
            //    return Ok(lstStf.GetRange(start, lstSize - start));
            //else
            //    return Ok(lstStf.GetRange(start, pageSize));
        }

        [HttpGet("GetStaffByCompanyId")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetStaffByCompanyId(
                string companyId
                //, int pageSize, int pageIndex
            )
        {
            ApplicationUser admin = await _userManager.GetUserAsync(HttpContext.User);
            List<InformationUser> lstStf = (from staff in _context.Users
                                            where staff.AdminId == admin.Id && staff.CompanyId == companyId
                                            select new InformationUser
                                            {
                                                Id = staff.Id,
                                                UserName = staff.UserName,
                                                FirstName = staff.FirstName,
                                                LastName = staff.LastName,
                                                Address = staff.Address,
                                                Role = staff.Role,
                                                PhoneNumber = staff.PhoneNumber,
                                                Email = staff.Email,
                                                DateOfBirth = staff.DateOfBirth.ToDate(),
                                                Gender = staff.Gender,
                                                Avatar = staff.Avatar,
                                                CompanyId = staff.CompanyId,
                                                Note = staff.Note,
                                                IsEnabled = staff.IsEnabled
                                            }).ToList();

            return Ok(lstStf);

            //int lstSize = lstStf.Count(),
            //    start = (pageIndex - 1) * pageSize;
            //if (start + pageSize > lstSize)
            //    return Ok(lstStf.GetRange(start, lstSize - start));
            //else
            //    return Ok(lstStf.GetRange(start, pageSize));
        }

        [HttpGet("GetStaffInfoById")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetStaffByAdmin(string staffId)
        {
            ApplicationUser staff = await _userManager.FindByIdAsync(staffId);
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
                Avatar = staff.Avatar,
                DateOfBirth = staff.DateOfBirth.ToDate(),
                Gender = staff.Gender,
                CompanyId = staff.CompanyId,
                Note = staff.Note,
                IsEnabled = staff.IsEnabled
            });
        }

        [HttpPut("BlockAndOpenAcount")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> BlockAndOpenAcount([FromBody] BlockAndOpenAcountModel blockAndOpenAcountModel)
        {
            ApplicationUser admin = await _userManager.GetUserAsync(HttpContext.User);
            ApplicationUser user = await _userManager.FindByIdAsync(blockAndOpenAcountModel.UserId);

            if (user.AdminId != admin.Id || user == null)
                return BadRequest("You are not allowed");

            user.IsEnabled = !user.IsEnabled;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Ok(true);
            else
                return BadRequest(result.Errors);
        }

        [HttpPut("UpdateAvatar")]
        [Authorize]
        public async Task<ActionResult> UpdateAvatar([FromForm] UpdateAvatarModel updateAvatarModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }
            try
            {
                ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);
                string strPicture = UploadFileToCloudinary.UploadImage(updateAvatarModel.FileAvatar);
                curUser.Avatar = strPicture;
                await _userManager.UpdateAsync(curUser);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpGet("GetSupervisor")]
        [Authorize(Roles = "Repair Person")]
        public async Task<ActionResult> GetSupervisorAsync()
        {
            ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);
            var result = (from supervisor in _context.Users
                          from request in _context.Requests
                          where request.CompanyId == curUser.CompanyId &&
                                supervisor.Id == request.SupervisorId
                          select new SupervisorInfo
                          {
                              SupervisorId = supervisor.Id,
                              SupervisorFirstName = supervisor.FirstName,
                              SupervisorLastName = supervisor.LastName,
                              SupervisorFullName = supervisor.LastName + " " + supervisor.FirstName,
                              SupervisorUserName = supervisor.UserName
                          }).Distinct().ToList();
            return Ok(result);
        }

    }
}