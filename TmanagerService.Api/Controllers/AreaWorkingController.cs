//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using TmanagerService.Api.Identity;
//using TmanagerService.Api.InputModels;
//using TmanagerService.Api.Models;
//using TmanagerService.Core.Entities;
//using TmanagerService.Core.Enums;
//using TmanagerService.Core.Extensions;
//using TmanagerService.Infrastructure.Data;

//namespace TmanagerService.Api.Controllers
//{
//    [Produces("application/json")]
//    [Route("api/AreaWorking")]
//    public class AreaWorkingController : ControllerBase
//    {
//        private readonly TmanagerServiceContext _context;
//        private readonly UserManager<ApplicationUser> _userManager;
//        public AreaWorkingController(
//            UserManager<ApplicationUser> userManager,
//            TmanagerServiceContext context
//            )
//        {
//            _userManager = userManager;
//            _context = context;
//        }

//        [HttpPost("AddAreaWorking")]
//        [Authorize(Roles = "Admin")]
//        public async Task<ActionResult> AddAreaWorking([FromBody] AddAreaWorkingModel addAreaWorkingModel)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState.GetErrors());
//            }

//            ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);

//            try
//            {
//                AreaWorking areaWorking = new AreaWorking
//                {
//                    AdminId = curUser.Id,
//                    AreaName = addAreaWorkingModel.AreaName,
//                    Status = true
//                };
//                await _context.AreaWorkings.AddAsync(areaWorking);
//                await _context.SaveChangesAsync();
//                string arWorkingId = _context.AreaWorkings.FirstOrDefault(a => a.AdminId == curUser.Id && a.AreaName == addAreaWorkingModel.AreaName).Id;
//                if (curUser.ListAreaWorkingId == null)
//                    curUser.ListAreaWorkingId = arWorkingId;
//                else
//                    curUser.ListAreaWorkingId = String_List.ToString(new List<string> { curUser.ListAreaWorkingId, arWorkingId });
//                await _userManager.UpdateAsync(curUser);
//                return Ok(true);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex);
//            }
//        }

//        [HttpPut("ChangeAreaWorkingStatus")]
//        [Authorize(Roles = "Admin")]
//        public async Task<ActionResult> ChangeAreaWorkingStatus(
//            [FromBody] ChangeAreaWorkingStatusModel changeAreaWorkingStatusModel)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState.GetErrors());
//            }

//            ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);
//            var areaWorking = (from areaW in _context.AreaWorkings
//                               where areaW.Id == changeAreaWorkingStatusModel.AreaWorkingId
//                                    && areaW.AdminId == curUser.Id
//                               select areaW).FirstOrDefault();
//            if (areaWorking == null)
//                return BadRequest("Your not allowed");

//            areaWorking.Status = !areaWorking.Status;
//            try
//            {
//                _context.AreaWorkings.Update(areaWorking);
//                await _context.SaveChangesAsync();
//                return Ok(true);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex);
//            }
//        }

//        [HttpPut("AddAreaWorkingForUser")]
//        [Authorize(Roles = "Admin")]
//        public async Task<ActionResult> AddAreaWorkingForUserAsync([FromBody] AddAreaWorkingForUserModel addAreaWorkingForUserModel)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState.GetErrors());
//            }

//            try
//            {
//                ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);

//                var listAreaWorkingId = (from area in _context.AreaWorkings
//                                         where area.AdminId == curUser.Id
//                                         select area.Id).ToList();
//                if (!listAreaWorkingId.Contains(addAreaWorkingForUserModel.AreaWorkingId))
//                    return BadRequest(addAreaWorkingForUserModel.AreaWorkingId + " is not in your area");
//                if (!_context.AreaWorkings.FirstOrDefault(a => a.Id == addAreaWorkingForUserModel.AreaWorkingId).Status)
//                    return BadRequest("This area working is disable");
//                ApplicationUser user = await _userManager.FindByIdAsync(addAreaWorkingForUserModel.UserId);
//                if (user == null)
//                    return BadRequest(addAreaWorkingForUserModel.UserId + " does not exist");
//                if (user.Role != RoleValues.Supervisor.ToDescription())
//                    return BadRequest("Cannot add to" + user.UserName);
//                if (user.ListAreaWorkingId == null)
//                    user.ListAreaWorkingId = addAreaWorkingForUserModel.AreaWorkingId;
//                else
//                {
//                    if (!String_List.ToList(user.ListAreaWorkingId).Contains(addAreaWorkingForUserModel.AreaWorkingId))
//                        user.ListAreaWorkingId = String_List.ToString(new List<string> { user.ListAreaWorkingId, addAreaWorkingForUserModel.AreaWorkingId });
//                    else
//                        return BadRequest(addAreaWorkingForUserModel.AreaWorkingId + " already exists");
//                }
//                await _userManager.UpdateAsync(user);
//                return Ok(true);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex);
//            }
//        }

//        [HttpPut("RemoveAreaWorkingForUser")]
//        [Authorize(Roles = "Admin")]
//        public async Task<ActionResult> RemoveAreaWorkingForUser([FromBody] RemoveAreaWorkingForUserModel removeAreaWorkingForUserModel)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState.GetErrors());
//            }

//            try
//            {
//                ApplicationUser user = await _userManager.FindByIdAsync(removeAreaWorkingForUserModel.UserId);
//                if (user.ListAreaWorkingId == null)
//                    return BadRequest("User haven't Area Working");

//                var listAreaUser = String_List.ToList(user.ListAreaWorkingId);
//                listAreaUser.Remove(removeAreaWorkingForUserModel.AreaWorkingId);
//                if (listAreaUser.Count == 0)
//                    user.ListAreaWorkingId = null;
//                else
//                    user.ListAreaWorkingId = String_List.ToString(listAreaUser);
//                await _userManager.UpdateAsync(user);
//                return Ok(true);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex);
//            }
//        }

//        [HttpGet("GetAreaValues")]
//        [Authorize(Roles = "Admin")]
//        public async Task<ActionResult> GetAreaValues()
//        {
//            ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);

//            var result = (from areaWorking in _context.AreaWorkings
//                          where areaWorking.AdminId == curUser.Id //|| areaWorking.AdminId == curUser.AdminId
//                          select new AreaWorkingValues
//                          {
//                              AreaId = areaWorking.Id,
//                              AdminId = areaWorking.AdminId,
//                              AreaName = areaWorking.AreaName,
//                              Status = areaWorking.Status
//                          }).ToList();

//            return Ok(result);
//        }

//    }
//}