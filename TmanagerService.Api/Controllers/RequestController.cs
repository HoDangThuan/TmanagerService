using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        DateTime now = DateTime.UtcNow;

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
            if (request != null)
            {
                if (request.SupervisorId == curUser.Id)
                {
                    try
                    {
                        request.TimeConfirm = now;
                        request.Status = RequestStatus.Approved.ToDescription();

                        _context.Requests.Update(request);
                        await _context.SaveChangesAsync();
                        return Ok(true);
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException(string.Join(Environment.NewLine, ex));
                    }
                }
                else
                    throw new ApplicationException(string.Join(Environment.NewLine, "you are not allowed"));
            }

            throw new ApplicationException(string.Join(Environment.NewLine, supervisorConfirmModel.RequestId + " does not exist"));
        }

        [HttpPut("RepairPersonFinish")]
        [Authorize(Roles = "Repair Person")]
        public async Task<ActionResult> RepairPersonFinish([FromBody] RepairPersonFinishModel repairPersonFinishModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }

            ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);
            var request = _context.Requests.FirstOrDefault(rq => rq.Id == repairPersonFinishModel.RequestId);
            if (request != null)
            {
                if(request.RepairPersonId == curUser.Id)
                {
                    try
                    {
                        request.TimeFinish = now;
                        request.Status = RequestStatus.Done.ToDescription();

                        request.PictureFinish = repairPersonFinishModel.ListPictureFinish;

                        _context.Requests.Update(request);
                        await _context.SaveChangesAsync();
                        return Ok(true);
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException(string.Join(Environment.NewLine, ex));
                    }
                }
                else
                    throw new ApplicationException(string.Join(Environment.NewLine, "you are not allowed"));
            }

            throw new ApplicationException(string.Join(Environment.NewLine, repairPersonFinishModel.RequestId + " does not exist"));
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
            if (request != null)
            {
                try
                {
                    request.RepairPerson = curUser;
                    request.TimeReceiveRequest = now;
                    request.Status = RequestStatus.ToDo.ToDescription();
                    _context.Requests.Update(request);
                    await _context.SaveChangesAsync();
                    return Ok(true);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(string.Join(Environment.NewLine, ex));
                }
            }

            throw new ApplicationException(string.Join(Environment.NewLine, repairPersonReceiveModel.RequestId + " does not exist"));
        }

        [HttpPost("InsertRequest")]
        [Authorize(Roles = "Supervisor")]
        public async Task<ActionResult> InsertRequest([FromBody] InsertRequestModel insertRequestModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrors());
            }

            ApplicationUser curUser = await _userManager.GetUserAsync(HttpContext.User);
            string requestId = Guid.NewGuid().ToString("N");
            var request = new Request
            {
                Id = requestId,
                Address = insertRequestModel.Address,
                Content = insertRequestModel.Content,
                Latlng_latitude = insertRequestModel.Latlng_latitude,
                Latlng_longitude = insertRequestModel.Latlng_longitude,
                TimeBeginRequest = now,
                PictureRequest = insertRequestModel.PictureRequest,
                Status = RequestStatus.Waiting.ToDescription(),
                Supervisor = curUser,
                Note = insertRequestModel.Note
            };
            try
            {
                await _context.Requests.AddAsync(request);
                await _context.SaveChangesAsync();
                return Ok(requestId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Join(Environment.NewLine, ex));
            }
        }

        [HttpGet("GetRequestByUserId")]
        [Authorize]
        public ActionResult GetRequestByUserId(string userId)
        {
            if (userId == null)
                return Ok(null);
            //throw new ApplicationException(string.Join(Environment.NewLine, "userId is null"));
            var result = (from request in _context.Requests
                          where request.RepairPersonId == userId || request.SupervisorId == userId
                          select new
                          {
                              id = request.Id,
                              address = request.Address,
                              content = request.Content,
                              latlng_latitude = request.Latlng_latitude,
                              latlng_longitude = request.Latlng_longitude,
                              timeBeginRequest = request.TimeBeginRequest,
                              timeReceiveRequest = request.TimeReceiveRequest,
                              timeFinish = request.TimeFinish,
                              timeConfirm = request.TimeConfirm,
                              pictureRequest = request.PictureRequest,
                              pictureFinish = request.PictureFinish,
                              status = request.Status,
                              supervisorId = request.SupervisorId,
                              repairPersonId = request.RepairPersonId,
                              note = request.Note
                          }).ToList();
            if (result is null)
                return Ok(null);
            //throw new ApplicationException(string.Join(Environment.NewLine,"Null data"));
            else
                return Ok(result);
        }

        [HttpGet("GetRequestById")]
        [Authorize]
        public ActionResult GetRequestById(string requestId)
        {
            if (requestId == null)
                return Ok(null);
            var result = (from request in _context.Requests
                          where request.Id == requestId
                          select new
                          {
                              id = request.Id,
                              address = request.Address,
                              content = request.Content,
                              latlng_latitude = request.Latlng_latitude,
                              latlng_longitude = request.Latlng_longitude,
                              timeBeginRequest = request.TimeBeginRequest,
                              timeReceiveRequest = request.TimeReceiveRequest,
                              timeFinish = request.TimeFinish,
                              timeConfirm = request.TimeConfirm,
                              pictureRequest = request.PictureRequest,
                              pictureFinish = request.PictureFinish,
                              status = request.Status,
                              supervisorId = request.SupervisorId,
                              repairPersonId = request.RepairPersonId,
                              note = request.Note
                          }).FirstOrDefault();
            if (result is null)
                return Ok(null);
            //throw new ApplicationException(string.Join(Environment.NewLine, requestId + " không tồn tại"));
            else
                return Ok(result);
        }

        [HttpGet("GetRequest")]
        [Authorize(Roles = "Admin")]
        public ActionResult GetRequest()
        {
            if (_context is null) return null;
            var result = (from request in _context.Requests
                          select new
                          {
                              id = request.Id
                          }).ToList();
            if (result is null)
                return null;
            else
                return Ok(result);
        }
    }
}