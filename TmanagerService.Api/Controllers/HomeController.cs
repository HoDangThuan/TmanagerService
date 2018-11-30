using Hangfire;
using Microsoft.AspNetCore.Mvc;
using TmanagerService.Infrastructure.Data;
using System.Linq;
using TmanagerService.Core.Enums;
using System;
using TmanagerService.Core.Extensions;
using TmanagerService.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TmanagerService.Api.Controllers
{
    public class HomeController : Controller
    {
        private readonly TmanagerServiceContext _context;

        public HomeController(
            TmanagerServiceContext context
            )
        {
            _context = context;
        }

        public IActionResult Index()
        {
            RecurringJob.AddOrUpdate(() => GetListRequestAsync(),
                    Startup.Configuration["utc_time_closed_request_CRON"]);
            return View();
        }

        public async Task GetListRequestAsync()
        {
            DateTime utcNow = DateTime.UtcNow;
            var lstRequest = (from request in _context.Requests
                          where request.Status == RequestStatus.Approved.ToDescription() &&
                                 utcNow.Subtract((DateTime)request.TimeConfirm).TotalDays >= 30
                          select request).ToList();

            foreach (var rq in lstRequest)
            {
                rq.Status = RequestStatus.Closed.ToDescription();
                _context.Requests.Update(rq);
            }
            await _context.SaveChangesAsync();
        }

    }
}
