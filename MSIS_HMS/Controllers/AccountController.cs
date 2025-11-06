using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MSIS_HMS.Models;
using MSIS_HMS.Interfaces;
using Microsoft.Extensions.Logging;
using MSIS_HMS.Infrastructure.Data;
using System.Linq;
using DevExpress.Data.ODataLinq.Helpers;
using Microsoft.EntityFrameworkCore;

namespace MSIS_HMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserService userService, ILogger<AccountController> logger,ApplicationDbContext context)
        {
            _userService = userService;
            _logger = logger;
            _context = context;
        } 

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] AuthenticateRequest model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.Login(model);
                if (result.Succeeded)
                {
                    if (model.UserName == "superadmin@gmail.com")
                    {
                        return RedirectToAction("Index", "Modules");
                    }
                    else
                    {
                        var userInfo = _context.Users.Where(x => x.UserName == model.UserName).FirstOrDefault();

                        var accessMenus = _context.UserAccessMenus.Include(x => x.Menu).Where(x => x.UserId == userInfo.Id && x.Menu.IsDashboard == true).FirstOrDefault();

                        return RedirectToAction(accessMenus.Menu.Action, accessMenus.Menu.Controller);
                    }
                 
                }
                else
                {
                    ViewData["Error"] = "The user name or password is incorrect";
                }
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            _userService.Logout();
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }
    }
}
