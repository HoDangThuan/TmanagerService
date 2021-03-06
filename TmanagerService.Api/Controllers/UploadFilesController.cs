﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TmanagerService.Api.Models;
using TmanagerService.Core.Extensions;

namespace TmanagerService.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/UploadFiles")]
    public class UploadFilesController : ControllerBase
    {
        [HttpPost]
        [Authorize]
        //[Authorize(Roles = "Supervisor, Repair Person")]
        public IActionResult UploadFiles(List<IFormFile> files)
        {
            if (files is null)
                return Ok(null);
            List<string> listURI = UploadFileToCloudinary.UploadListImage(files);
            string result = listURI.ListToString();
            return Ok(result);
        }
    }
}