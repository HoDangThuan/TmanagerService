using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TmanagerService.Api.Models;

namespace TmanagerService.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/UploadFiles")]
    public class UploadFilesController : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Supervisor, Repair Person")]
        public IActionResult UploadFiles(List<IFormFile> files)
        {
            if (files is null)
                return Ok(null);
            List<string> listURI = UploadFileToCloudinary.UploadListImage(files);
            string result = string.Join(", ", listURI);
            return Ok(result);
        }
    }
}