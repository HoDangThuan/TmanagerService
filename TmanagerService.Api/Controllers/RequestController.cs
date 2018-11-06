using System;
using System.Collections.Generic;
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
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {

        private readonly TmanagerServiceContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RequestController(
            TmanagerServiceContext context,
            UserManager<ApplicationUser> userManager
            )
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("InsertRequest")]
        [Authorize(Roles = "Supervisor")]
        public async Task<ActionResult> InsertRequest([FromForm] InsertRequestModel insertRequestModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }

            if (insertRequestModel.PictureRequest == null)
                return BadRequest("Picture not null");
            if (insertRequestModel.PictureRequest.Count > 5)
                return BadRequest("Cannot upload more than 5 picture");
            string strPictureRequest = UploadFileToCloudinary.UploadListImage(insertRequestModel.PictureRequest).ListToString();
           
            ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);

            Company company = (from cmp in _context.Companys
                               where cmp.Id == insertRequestModel.CompanyId
                               select cmp).FirstOrDefault();
            if (company == null)
                return BadRequest("Company assignto required");
            DateTime now = DateTime.Now;
            var request = new Request
            {
                //Id = requestId,
                Address = insertRequestModel.Address,
                Content = insertRequestModel.Content,
                Latlng_latitude = insertRequestModel.Latlng_latitude,
                Latlng_longitude = insertRequestModel.Latlng_longitude,
                TimeBeginRequest = now,
                PictureRequest = strPictureRequest,
                Status = RequestStatus.Waiting.ToDescription(),
                Supervisor = curUser,
                Company = company
            };
            try
            {
                await _context.Requests.AddAsync(request);
                await _context.SaveChangesAsync();
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("RepairPersonReceive")]
        [Authorize(Roles = "Repair Person")]
        public async Task<ActionResult> RepairPersonReceive([FromBody] RepairPersonReceiveModel repairPersonReceiveModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }
            
            ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);
            var request = _context.Requests.FirstOrDefault(rq => rq.Id == repairPersonReceiveModel.RequestId);
            if (request.Status != RequestStatus.Waiting.ToDescription())
                return BadRequest("Cannot Receive");
            if (request.CompanyId != curUser.CompanyId)
                return BadRequest("Cannot Receive");
            if (request != null)
            {
                try
                {
                    DateTime now = DateTime.Now;
                    request.RepairPerson = curUser;
                    request.TimeReceiveRequest = now;
                    request.Status = RequestStatus.ToDo.ToDescription();
                    _context.Requests.Update(request);
                    await _context.SaveChangesAsync();
                    return Ok(true);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }

            return BadRequest(repairPersonReceiveModel.RequestId + " does not exist");
        }

        [HttpPut("RepairPersonFinish")]
        [Authorize(Roles = "Repair Person")]
        public async Task<ActionResult> RepairPersonFinish([FromForm] RepairPersonFinishModel repairPersonFinishModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }
            ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);
            var request = _context.Requests.FirstOrDefault(rq => rq.Id == repairPersonFinishModel.RequestId);
            if (request.Status != RequestStatus.ToDo.ToDescription())
                return BadRequest("Cannot Finish");
            if(request.RepairPersonId != curUser.Id)
                return BadRequest("Cannot Finish");
            if (request != null)
            {
                if (request.RepairPersonId == curUser.Id)
                {
                    try
                    {
                        if (repairPersonFinishModel.ListPictureFinish.Count > 5)
                            return BadRequest("Cannot upload more than 5 picture");
                        string strPictureFinish = UploadFileToCloudinary.UploadListImage(repairPersonFinishModel.ListPictureFinish).ListToString();
                        DateTime now = DateTime.Now;
                        request.TimeFinish = now;
                        request.Status = RequestStatus.Done.ToDescription();

                        request.PictureFinish = strPictureFinish;

                        _context.Requests.Update(request);
                        await _context.SaveChangesAsync();
                        return Ok(true);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex);
                    }
                }
                else
                    return BadRequest("you are not allowed");
            }

            return BadRequest(repairPersonFinishModel.RequestId + " does not exist");
        }

        [HttpPut("SupervisorConfirm")]
        [Authorize(Roles = "Supervisor")]
        public async Task<ActionResult> SupervisorConfirm([FromBody] SupervisorConfirmModel supervisorConfirmModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }

            ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);
            var request = _context.Requests.FirstOrDefault(rq => rq.Id == supervisorConfirmModel.RequestId);
            if (request.Status != RequestStatus.Done.ToDescription())
                return BadRequest("Cannot Approved");
            if (request.SupervisorId != curUser.Id)
                return BadRequest("Cannot Finish");
            if (request != null)
            {
                if (request.SupervisorId == curUser.Id)
                {
                    try
                    {
                        DateTime now = DateTime.Now;
                        request.TimeConfirm = now;
                        request.Status = RequestStatus.Approved.ToDescription();

                        _context.Requests.Update(request);
                        await _context.SaveChangesAsync();
                        return Ok(true);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex);
                    }
                }
                else
                    return BadRequest("you are not allowed");
            }

            return BadRequest(supervisorConfirmModel.RequestId + " does not exist");
        }

        [HttpGet("GetRequestById")]
        [Authorize]
        public ActionResult GetRequestById(string requestId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }

            var result = (from request in _context.Requests
                          where request.Id == requestId
                          select new ReturnRequest
                          {
                              Id = request.Id,
                              Address = request.Address,
                              Latlng_latitude = request.Latlng_latitude,
                              Latlng_longitude = request.Latlng_longitude,
                              SupervisorId = request.SupervisorId,
                              SupervisorName = (from spv in _context.Users
                                                where spv.Id == request.SupervisorId
                                                select spv.LastName.FullName(spv.FirstName)).FirstOrDefault(),
                              RepairPersonId = request.RepairPersonId,
                              RepairPersonName = (from rps in _context.Users
                                                  where rps.Id == request.RepairPersonId
                                                  select rps.LastName.FullName(rps.FirstName)).FirstOrDefault(),
                              Content = request.Content,
                              Status = request.Status,
                              TimeBeginRequest = request.TimeBeginRequest.ToDateTimeFull(),
                              TimeReceiveRequest = request.TimeReceiveRequest.ToDateTimeFull(),
                              TimeFinish = request.TimeFinish.ToDateTimeFull(),
                              TimeConfirm = request.TimeConfirm.ToDateTimeFull(),
                              PictureRequest = request.PictureRequest.StringToList(),
                              PictureFinish = request.PictureFinish.StringToList(),
                              Note = request.Note
                          }).FirstOrDefault();
            return Ok(result);
        }

        [HttpGet("GetRequest")]
        [Authorize]
        public async Task<ActionResult> GetRequestAsync()
        {
            ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);
            List<ReturnRequest> result = new List<ReturnRequest>();            
            if ( curUser.Role == RoleValues.Admin.ToDescription() )
            {
                List<string> listCompany = (from company in _context.Companys
                                            where company.AdminId == curUser.Id
                                            select company.Id).ToList();
                result = (from request in _context.Requests
                          where listCompany.Contains(request.CompanyId)
                          select new ReturnRequest
                          {
                              Id = request.Id,
                              Address = request.Address,
                              Latlng_latitude = request.Latlng_latitude,
                              Latlng_longitude = request.Latlng_longitude,
                              SupervisorId = request.SupervisorId,
                              SupervisorName = (from spv in _context.Users
                                                where spv.Id == request.SupervisorId
                                                select spv.LastName.FullName(spv.FirstName)).FirstOrDefault(),
                              RepairPersonId = request.RepairPersonId,
                              RepairPersonName = (from rps in _context.Users
                                                  where rps.Id == request.RepairPersonId
                                                  select rps.LastName.FullName(rps.FirstName)).FirstOrDefault(),
                              Content = request.Content,
                              Status = request.Status,
                              TimeBeginRequest = request.TimeBeginRequest.ToDateTimeFull(),
                              TimeReceiveRequest = request.TimeReceiveRequest.ToDateTimeFull(),
                              TimeFinish = request.TimeFinish.ToDateTimeFull(),
                              TimeConfirm = request.TimeConfirm.ToDateTimeFull(),
                              PictureRequest = request.PictureRequest.StringToList(),
                              PictureFinish = request.PictureFinish.StringToList(),
                              Note = request.Note
                          }).ToList();
            }
            if (curUser.Role == RoleValues.RepairPerson.ToDescription())
            {
                result = (from request in _context.Requests
                          where request.CompanyId == curUser.CompanyId && 
                                request.Status != RequestStatus.Approved.ToDescription()
                          select new ReturnRequest
                          {
                              Id = request.Id,
                              Address = request.Address,
                              Latlng_latitude = request.Latlng_latitude,
                              Latlng_longitude = request.Latlng_longitude,
                              SupervisorId = request.SupervisorId,
                              SupervisorName = (from spv in _context.Users
                                                where spv.Id == request.SupervisorId
                                                select spv.LastName.FullName(spv.FirstName)).FirstOrDefault(),
                              RepairPersonId = request.RepairPersonId,
                              RepairPersonName = (from rps in _context.Users
                                                  where rps.Id == request.RepairPersonId
                                                  select rps.LastName.FullName(rps.FirstName)).FirstOrDefault(),
                              Content = request.Content,
                              Status = request.Status,
                              TimeBeginRequest = request.TimeBeginRequest.ToDateTimeFull(),
                              TimeReceiveRequest = request.TimeReceiveRequest.ToDateTimeFull(),
                              TimeFinish = request.TimeFinish.ToDateTimeFull(),
                              TimeConfirm = request.TimeConfirm.ToDateTimeFull(),
                              PictureRequest = request.PictureRequest.StringToList(),
                              PictureFinish = request.PictureFinish.StringToList(),
                              Note = request.Note
                          }).ToList();
            }
            if (curUser.Role == RoleValues.Supervisor.ToDescription())
            {
                result = (from request in _context.Requests
                          where request.SupervisorId == curUser.Id &&
                                request.Status != RequestStatus.Approved.ToDescription()
                          select new ReturnRequest
                          {
                              Id = request.Id,
                              Address = request.Address,
                              Latlng_latitude = request.Latlng_latitude,
                              Latlng_longitude = request.Latlng_longitude,
                              SupervisorId = request.SupervisorId,
                              SupervisorName = (from spv in _context.Users
                                                where spv.Id == request.SupervisorId
                                                select spv.LastName.FullName(spv.FirstName)).FirstOrDefault(),
                              RepairPersonId = request.RepairPersonId,
                              RepairPersonName = (from rps in _context.Users
                                                  where rps.Id == request.RepairPersonId
                                                  select rps.LastName.FullName(rps.FirstName)).FirstOrDefault(),
                              Content = request.Content,
                              Status = request.Status,
                              TimeBeginRequest = request.TimeBeginRequest.ToDateTimeFull(),
                              TimeReceiveRequest = request.TimeReceiveRequest.ToDateTimeFull(),
                              TimeFinish = request.TimeFinish.ToDateTimeFull(),
                              TimeConfirm = request.TimeConfirm.ToDateTimeFull(),
                              PictureRequest = request.PictureRequest.StringToList(),
                              PictureFinish = request.PictureFinish.StringToList(),
                              Note = request.Note
                          }).ToList();
            }
            return Ok(result);
        }

        [HttpPut("Report")]
        [Authorize(Roles = "Supervisor, Repair Person")]
        public async Task<ActionResult> ReportAsync(ReportModel reportModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }

            ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);
            var request = _context.Requests.FirstOrDefault(rq => rq.Id == reportModel.RequestId);
            if (request.ReportUserId != null)
                return BadRequest("cannot report");
            if (request != null)
            {
                try
                {
                    DateTime now = DateTime.Now;
                    request.ReportContent = reportModel.ContentReport;
                    request.ReportUser = curUser;

                    _context.Requests.Update(request);
                    await _context.SaveChangesAsync();
                    return Ok(true);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }

            return BadRequest(reportModel.RequestId + " does not exist");
        }
    }
}