//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using TmanagerService.Core.Enums;
//using TmanagerService.Core.Extensions;

//namespace TmanagerService.Api.Controllers
//{
//    [Route("api/Extensions")]
//    [ApiController]
//    public class ExtensionsController : ControllerBase
//    {
//        [HttpGet("GetRequestStatus")]
//        [Authorize]
//        public ActionResult GetRequestStatus()
//        {
//            List<string> lstRequestStatus = new List<string>
//            {
//                RequestStatus.Waiting.ToDescription(),
//                RequestStatus.ToDo.ToDescription(),
//                RequestStatus.Done.ToDescription(),
//                RequestStatus.Approved.ToDescription()
//            };

//            return Ok(lstRequestStatus);
//        }
//        [HttpGet("GetClaims")]
//        [Authorize]
//        public ActionResult GetClaims()
//        {
//            List<string> lstClaims = new List<string>
//            {
//                Claim.CreateAccount.ToDescription(),
//                Claim.InsertRequest.ToDescription(),
//                Claim.ReceiveRequest.ToDescription()
//            };

//            return Ok(lstClaims);
//        }
//        [HttpGet("GetRoles")]
//        [Authorize]
//        public ActionResult GetRoles()
//        {
//            List<string> lstRoles = new List<string>
//            {
//                RoleValues.Admin.ToDescription(),
//                RoleValues.Supervisor.ToDescription(),
//                RoleValues.RepairPerson.ToDescription()
//            };

//            return Ok(lstRoles);
//        }
//        [HttpGet("GetGender")]
//        [Authorize]
//        public ActionResult GetGender()
//        {
//            List<string> lstGender = new List<string>
//            {
//                Gender.Male.ToDescription(),
//                Gender.Female.ToDescription()
//            };

//            return Ok(lstGender);
//        }

//        [HttpGet("ListToJsonObject")]
//        [Authorize]
//        public ActionResult ListToJsonObject(string listString)
//        {
//            return Ok(listString.Split(", "));
//        }

//        [HttpGet("Demo")]
//        public ActionResult Demo()
//        {
//            return Ok("Wellcome");
//        }
//    }
//}