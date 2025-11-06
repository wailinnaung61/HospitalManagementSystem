using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using System.Security.Claims;
using MSIS_HMS.Infrastructure.Enums;
using System.Linq;

namespace MSIS_HMS.Components
{
    [ViewComponent(Name = "TopbarMenu")]
    public class TopbarMenuViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public TopbarMenuViewComponent(UserManager<ApplicationUser> userManager)
        {           
            _userManager = userManager;
        }

        public IViewComponentResult Invoke()
        {
            ViewData["UserName"] = _userManager.GetUserName((ClaimsPrincipal)User);
            return View();
        }
    }
}
