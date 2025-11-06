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
    [ViewComponent(Name = "SidebarMenu")]
    public class SidebarMenuViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMenuRepository _menuRepository;

        public SidebarMenuViewComponent(UserManager<ApplicationUser> userManager, IMenuRepository menuRepository)
        {
            _userManager = userManager;
            _menuRepository = menuRepository;
        }

        public IViewComponentResult Invoke()
        {
            string userId = null;
            if (!User.IsInRole(RoleEnum.Role.Superadmin.ToDescription()))
            {
                userId = _userManager.GetUserId((ClaimsPrincipal)User);
            }
            var menus = _menuRepository.GetUserAccessList(userId).Where(x => x.IsActive).ToList();
            return View(menus);
        }
    }
}
