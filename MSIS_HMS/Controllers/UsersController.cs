using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Enums;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Interfaces;
using MSIS_HMS.Models;
using MSIS_HMS.Models.ViewModels;
using Newtonsoft.Json;
using X.PagedList;
using EnumExtension = MSIS_HMS.Infrastructure.Enums.EnumExtension;

namespace MSIS_HMS.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IBranchRepository _branchRepository;
        private readonly IOutletRepository _outletRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly Pagination _pagination;

        public UsersController(ApplicationDbContext context, IUserService userService, IBranchRepository branchRepository, IOutletRepository outletRepository, IMenuRepository menuRepository,IOptions<Pagination> pagination)
        {
            _context = context;
            _userService = userService; 
            _branchRepository = branchRepository;
            _outletRepository = outletRepository;
            _menuRepository = menuRepository;
            _pagination = pagination.Value;
        }

        public List<UserAccessMenu> Initialize(RegisterViewModel model = null)
        {
            int? doctorId = null, branchId = null, outletId = null;
            string role = null;
            if (model != null)
            {
                doctorId = model.DoctorId;
                role = model.Role;
                branchId = model.BranchId;
                outletId = model.OutletId;
            }

            var menus = new List<Menu>();
            var branches = _branchRepository.GetAll();
            ViewData["Branches"] = new SelectList(branches, "Id", "Name", branchId);
            var outlets = _outletRepository.GetAll(branchId ?? null).OrderBy(x => x.Name);
            ViewData["Outlets"] = new SelectList(outlets, "Id", "Name", outletId);
            menus = _menuRepository.GetParentList(true);
            ViewData["Menus"] = menus;
            var userAccessMenus = new List<UserAccessMenu>();
            if (model != null)
            {
                menus.ForEach(m =>
                {
                    m.ChildMenus.ToList().ForEach(x =>
                    {
                        x.Selected = model.UserAccessMenus.Where(x => x.Selected).Select(x => x.MenuId).Contains(x.Id);
                        userAccessMenus.Add(new UserAccessMenu { MenuId = x.Id, Selected = x.Selected });
                    });
                });
            }
            ViewData["Menus"] = menus;
            var doctors = _context.Doctors.Where(x => !x.IsDelete).ToList();
            ViewData["Doctors"] = new SelectList(doctors, "Id", "Name", doctorId);
            var roles = _context.Roles.Where(x => x.Name != EnumExtension.ToDescription(RoleEnum.Role.Superadmin)).ToList();
            ViewData["Roles"] = new SelectList(roles, "Name", "Name", role);

            return userAccessMenus;
        }

        // GET: Users
        public ActionResult Index(int? page=1,string UserName=null,string RoleId=null)
        {
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            string role = null;
            var roles = _context.Roles.Where(x => x.Name != EnumExtension.ToDescription(RoleEnum.Role.Superadmin)).ToList();
            ViewData["Roles"] = new SelectList(roles, "Id", "Name", role);
            var users = _userService.GetAll().Where(user =>
                (UserName != null ? (user.UserName.ToLower().Contains(UserName.ToLower()) || UserName.ToLower().Contains(user.UserName.ToLower())) : true) &&
                (RoleId != null ? user.RoleId == RoleId : true)).ToList();
            return View(users.Where(x => x.RoleName != EnumExtension.ToDescription(RoleEnum.Role.Superadmin)).ToList().ToPagedList((int)page, pageSize));
        }

        // GET: Users/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Users/Create
        public ActionResult Create(int? BranchId=null)
        {
            Initialize(null);
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var user = await _userService.Register(model);
                        if (user != null)
                        {
                            await transaction.CommitAsync();
                            TempData["notice"] = Enums.StatusEnum.NoticeStatus.Success;
                            return RedirectToAction(nameof(UsersController.Index), "Users");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        await transaction.RollbackAsync();
                    }
                }
            }

            Initialize(model);
            return View();
        }

        // GET: Users/Edit/5
        public ActionResult Edit(string id)
        {
            var user = _userService.Get(id);
            var model = _userService.ConvertModel(user);
            var userAccessMenus = Initialize(model);
            model.UserAccessMenus = userAccessMenus;
            return View(model);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, RegisterViewModel model)
        {
            try
            {
                ModelState.Remove("Password");
                if (ModelState.IsValid)
                {
                    if (id != model.Id)
                    {
                        return BadRequest();
                    }
                    var _user = await _userService.UpdateAsync(model);
                    if (_user != null)
                    {
                        TempData["notice"] = Enums.StatusEnum.NoticeStatus.Success;
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            var userAccessMenus = Initialize(model);
            model.UserAccessMenus = userAccessMenus;
            return View(model);
        }

        // GET: Users/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var succeed = await _userService.DeleteAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}