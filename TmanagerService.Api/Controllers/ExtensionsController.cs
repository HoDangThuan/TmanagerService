using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TmanagerService.Core.Enums;
using TmanagerService.Core.Extensions;

namespace TmanagerService.Api.Controllers
{
    [Route("api/Extensions")]
    [ApiController]
    public class ExtensionsController : ControllerBase
    {
        [HttpGet("GetGender")]
        public ActionResult GetGender()
        {
            List<string> lstGender = new List<string>
            {
                Gender.Male.ToDescription(),
                Gender.Female.ToDescription()
            };

            return Ok(lstGender);
        }

        [HttpGet("ListToJsonObject")]
        public ActionResult ListToJsonObject(string listString)
        {
            return Ok(listString.Split(", "));
        }
    }
}