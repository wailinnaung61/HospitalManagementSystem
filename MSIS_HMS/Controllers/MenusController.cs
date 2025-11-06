using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Vml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Enums;
using MSIS_HMS.Helpers;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Models;
using X.PagedList;
using static MSIS_HMS.Infrastructure.Enums.DbEnum;
using Path = System.IO.Path;

namespace MSIS_HMS.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Superadmin")]
    public class MenusController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMenuRepository _menuRepository;
        private readonly IModuleRepository _moduleRepository;
        private readonly Pagination _pagination;
        private readonly ILogger<MenusController> _logger;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ApplicationDbContext _context;


        public MenusController(UserManager<ApplicationUser> userManager, IMenuRepository menuRepository, IModuleRepository moduleRepository, IOptions<Pagination> pagination, ILogger<MenusController> logger, IWebHostEnvironment hostEnvironment, ApplicationDbContext context)
        {
            _userManager = userManager;
            _menuRepository = menuRepository;
            _moduleRepository = moduleRepository;
            _pagination = pagination.Value;
            _logger = logger;
            webHostEnvironment = hostEnvironment;
            _context = context;
        }

        public void Initialize(Menu menu = null)
        {
            int? id = null, moduleId = null, parentMenuId = null;
            if (menu != null)
            {
                id = menu.Id;
                moduleId = menu.ModuleId;
                parentMenuId = menu.ParentId;
            }
            var modules = _moduleRepository.GetAll();
            ViewData["Modules"] = new SelectList(modules, "Id", "Name", moduleId);
            var parentMenus = _menuRepository.GetParentList().Where(x => x.Id != id);
            ViewData["ParentMenus"] = new SelectList(parentMenus, "Id", "Name", parentMenuId);
            var groupMenus = _menuRepository.GetGroupList().Where(x => x.Id != id);
            ViewData["GroupMenus"] = new SelectList(groupMenus, "Id", "Name", menu?.GroupId);
        }

        // GET: Menus
        public ActionResult Index(int? page = 1, string MenuName = null, string DisplayName = null)
        {
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            var parentMenus = _menuRepository.GetParentList(true);
            return View(parentMenus.OrderByDescending(menu => menu.MenuOrder).ToList());
        }

        // GET: Menus/Create
        public ActionResult Create()
        {
            var menu = new Menu();
            if (menu.Image != null)
            {
                string iconFolderPath = Path.Combine(webHostEnvironment.WebRootPath, menu.Image);
                byte[] imageArray = System.IO.File.ReadAllBytes(iconFolderPath);
                menu.ImageContent = "data:image/png;base64, " + Convert.ToBase64String(imageArray);
            }
            Initialize();
            return View();
        }

        // POST: Menus/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Menu menu)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                string iconFolderPath = Path.Combine(webHostEnvironment.WebRootPath, "images/menuicons/");
                if(menu.ImageFile == null)
                {
                    return RedirectToAction("Create");
                }
                var getUniqueFileName = menu.ImageFile.GetUniqueName();
                menu.Image = "images/menuicons/" + getUniqueFileName;
                var uniqueFilename = menu.ImageFile != null ? iconFolderPath + getUniqueFileName : "";

                try
                {
                    if (ModelState.IsValid)
                    {
                        var didUploaded = false;
                        if (menu.ImageFile != null)
                        {
                            using (var fileStream = new FileStream(uniqueFilename, FileMode.Create))
                            {
                                menu.ImageFile.CopyTo(fileStream);
                            }
                            if (System.IO.File.Exists(uniqueFilename))
                            {
                                didUploaded = true;
                            }
                        }
                        if (didUploaded)
                        {
                            menu = await _userManager.AddUserAndTimestamp(menu, User, DbActionEnum.Create);
                            var _menu = await _menuRepository.AddAsync(menu);
                            await transaction.CommitAsync();
                            if (_menu != null)
                            {
                                TempData["notice"] = Enums.StatusEnum.NoticeStatus.Success;
                                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));
                                return RedirectToAction(nameof(Index));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("Not successful save Blog");
                    Console.WriteLine(e.Message);
                    await transaction.RollbackAsync();
                    string filePath = Path.Combine(iconFolderPath, uniqueFilename);
                    DeleteImageInStaticFile(filePath);
                    Initialize(menu);
                    return View(menu);
                }
            }
            Initialize(menu);
            return View();
        }

        // GET: Menus/Edit/5
        public ActionResult Edit(int id)
        {
            var menu = _menuRepository.Get(id);
            if (menu.Image != null)
            {
                string iconFolderPath = Path.Combine(webHostEnvironment.WebRootPath, menu.Image);
                byte[] imageArray = System.IO.File.ReadAllBytes(iconFolderPath);
                menu.ImageContent = "data:image/png;base64, " + Convert.ToBase64String(imageArray);
            }
            Initialize(menu);
            return View(menu);
        }

        // POST: Menus/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(int id, Menu menu)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var didUploaded = false;

                        string iconFolderPath = Path.Combine(webHostEnvironment.WebRootPath, "images/menuicons/");
                        if (menu.ImageFile == null)
                        {
                            didUploaded = true;
                            if (didUploaded)
                            {
                                menu = await _userManager.AddUserAndTimestamp(menu, User, DbActionEnum.Update);
                                var _menu = await _menuRepository.UpdateAsync(menu);
                                await transaction.CommitAsync();
                                if (_menu != null)
                                {
                                    TempData["notice"] = Enums.StatusEnum.NoticeStatus.Success;
                                    _logger.LogInformation(
                                        Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                                    return RedirectToAction(nameof(Index));
                                }
                            }
                        }
                        var getUniqueFileName = menu.ImageFile.GetUniqueName();
                            var uniqueFilename = menu.ImageFile != null ? iconFolderPath + getUniqueFileName : "";
                        if (ModelState.IsValid)
                        {
                            if (menu.ImageFile != null)
                            {
                                using (var fileStream = new FileStream(uniqueFilename, FileMode.Create))
                                {
                                    menu.ImageFile.CopyTo(fileStream);
                                }

                                if (System.IO.File.Exists(uniqueFilename))
                                {
                                    if (menu.Image != null)
                                    {
                                        DeleteImageInStaticFile(Path.Combine(webHostEnvironment.WebRootPath, menu.Image));
                                    }
                                    menu.Image = "images/menuicons/" + getUniqueFileName;
                                    didUploaded = true;
                                }
                            }
                            if (didUploaded)
                            {
                                menu = await _userManager.AddUserAndTimestamp(menu, User, DbActionEnum.Update);
                                var _menu = await _menuRepository.UpdateAsync(menu);
                                await transaction.CommitAsync();
                                if (_menu != null)
                                {
                                    TempData["notice"] = Enums.StatusEnum.NoticeStatus.Success;
                                    _logger.LogInformation(
                                        Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                                    return RedirectToAction(nameof(Index));
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Not successful save Blog");
                        Console.WriteLine(e.Message);
                        await transaction.RollbackAsync();
                        string filePath = Path.Combine(webHostEnvironment.WebRootPath, menu.Image);
                        DeleteImageInStaticFile(filePath);
                        Initialize(menu);
                        return View(menu);
                    }
                }
            }
            Initialize(menu);
            return View();
        }

        // GET: Menus/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var menu = _menuRepository.Get(id);
                DeleteImageInStaticFile(Path.Combine(webHostEnvironment.WebRootPath, menu.Image));
                var succeed = await _menuRepository.DeleteAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
                _logger.LogInformation(
                    Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Delete));
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Menus/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                var menu = _menuRepository.Get(id);
                string iconFolderPath = Path.Combine(webHostEnvironment.WebRootPath, menu.Image);
                DeleteImageInStaticFile(iconFolderPath);
                var _doctor = await _menuRepository.DeleteAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Delete));
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }
            return RedirectToAction(nameof(Index));
        }

        public void DeleteImageInStaticFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
        
    }
}
