using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MSIS_HMS.Infrastructure.Helpers;

namespace MSIS_HMS.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetDaysOfWeekSelectList()
        {
            return Ok(DateTimeExtensions.GetDaysOfWeekSelectList().Select(x => new { label = x.Text, value = x.Value }));
        }
    }
}