using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TmanagerService.Api.Identity;
using TmanagerService.Api.InputModels;
using TmanagerService.Api.Models;
using TmanagerService.Core.Entities;
using TmanagerService.Core.Enums;
using TmanagerService.Core.Extensions;
using TmanagerService.Infrastructure.Data;

namespace TmanagerService.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Company")]
    public class CompanyController : ControllerBase
    {
        private readonly TmanagerServiceContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CompanyController(
            UserManager<ApplicationUser> userManager,
            TmanagerServiceContext context
            )
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost("AddCompany")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddCompany([FromBody] AddCompanyModel addCompanyModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }

            ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);

            try
            {
                Company company = new Company
                {
                    AdminId = curUser.Id,
                    Name = addCompanyModel.CompanyName,
                    Address = addCompanyModel.Address,
                    IsDepartmentOfConstruction = addCompanyModel.IsDepartmentOfConstruction,
                    Status = true
                };
                await _context.Companys.AddAsync(company);
                await _context.SaveChangesAsync();
                //string companyId = _context.Companys.FirstOrDefault(c => c.AdminId == curUser.Id && c.Name == addCompanyModel.CompanyName).Id;
                //if (curUser.Company == null)
                //    curUser.ListAreaWorkingId = arWorkingId;
                //else
                //    curUser.ListAreaWorkingId = String_List.ToString(new List<string> { curUser.ListAreaWorkingId, arWorkingId });
                //await _userManager.UpdateAsync(curUser);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("ChangeCompanyStatus")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ChangeCompanyStatusAsync([FromBody] ChangeCompanyStatusModel changeCompanyStatusModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }

            ApplicationUser admin = await _userManager.GetUserAsync(HttpContext.User);
            var company = (from cpn in _context.Companys
                           where cpn.Id == changeCompanyStatusModel.CompanyId
                                && cpn.AdminId == admin.Id
                           select cpn).FirstOrDefault();
            if (company == null)
                return BadRequest("Your not allowed");

            try
            {
                company.Status = !company.Status;
                _context.Companys.Update(company);
                await _context.SaveChangesAsync();
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("AddCompanyForUser")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddCompanyForUserAsync([FromBody] AddCompanyForUserModel addAreaWorkingForUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }

            try
            {
                ApplicationUser user = await _userManager.FindByIdAsync(addAreaWorkingForUserModel.UserId);
                Company company = _context.Companys.FirstOrDefault(c => c.Id == addAreaWorkingForUserModel.CompanyId);
                user.Company = company;
                await _userManager.UpdateAsync(user);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("RemoveCompanyForUser")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RemoveCompanyForUser([FromBody] RemoveCompanyForUserModel removeCompanyForUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }

            try
            {
                ApplicationUser user = await _userManager.FindByIdAsync(removeCompanyForUserModel.UserId);
                user.CompanyId = null;
                user.Company = null;
                await _userManager.UpdateAsync(user);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("GetAllCompany")]
        [Authorize(Roles = "Admin, Supervisor")]
        public async Task<ActionResult> GetAllCompany()
        {
            ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);
            var lstCompany = (from company in _context.Companys
                              where
                                (curUser.Role == RoleValues.Admin.ToDescription() && company.AdminId == curUser.Id) ||
                                (curUser.Role == RoleValues.Supervisor.ToDescription() && company.AdminId == curUser.AdminId && !company.IsDepartmentOfConstruction && company.Status)
                              orderby company.IsDepartmentOfConstruction descending
                              select new CompanyReturn
                              {
                                  Id = company.Id,
                                  Name = company.Name,
                                  Address = company.Address,
                                  Status = company.Status
                              }).ToList();
            return Ok(lstCompany);
        }

        [HttpPut("ChangeInformationCompany")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ChangeInformationCompany([FromBody] ChangeInformationCompanyModel changeInformationCompanyModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }

            try
            {
                Company company = _context.Companys.FirstOrDefault(c => c.Id == changeInformationCompanyModel.CompanyId);
                if (changeInformationCompanyModel.Address != null)
                    company.Address = changeInformationCompanyModel.Address;
                if (changeInformationCompanyModel.Name != null)
                    company.Name = changeInformationCompanyModel.Name;

                _context.Companys.Update(company);
                await _context.SaveChangesAsync();
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}