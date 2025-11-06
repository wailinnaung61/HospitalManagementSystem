using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Enums;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Interfaces;
using MSIS_HMS.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using X.PagedList;

namespace MSIS_HMS.Controllers
{
    public class BedTypesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBedTypeRepository _bedTypeRepository;
        private readonly IUserService _userService;
        private readonly Pagination _pagination;

        public BedTypesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IUserService userService, IBedTypeRepository bedTypeRepository, IOptions<Pagination> pagination)
        {
            _context = context;
            _userManager = userManager;
            _pagination = pagination.Value;
            _bedTypeRepository = bedTypeRepository;
            _userService = userService;
        }

        // GET
        public IActionResult Index(int? page = 1, string BedTypeName = null)
        {
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            var bedType = _bedTypeRepository.GetAll().Where(bedType =>
                ((BedTypeName != null && bedType.Name.ToLower().Contains(BedTypeName.ToLower())) || (BedTypeName == null && bedType.Name != null))).ToList();

            return View(bedType.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BedType bedType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bedType = await _userManager.AddUserAndTimestamp(bedType, User, DbEnum.DbActionEnum.Create);
                    var _bedType = await _bedTypeRepository.AddAsync(bedType);
                    if (_bedType != null)
                    {
                        TempData["notice"] = StatusEnum.NoticeStatus.Success;
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return View();
        }

        public ActionResult Edit(int id)
        {
            var _bedTypes = _bedTypeRepository.Get(id);
            return View(_bedTypes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(BedType bedType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bedType = await _userManager.AddUserAndTimestamp(bedType, User, DbEnum.DbActionEnum.Update);
                    var _bedType = await _bedTypeRepository.UpdateAsync(bedType);
                    TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return View(bedType);
        }

        public async Task<ActionResult> Delete(int id)
        {
            var _bedType = await _bedTypeRepository.DeleteAsync(id);
            TempData["notice"] = StatusEnum.NoticeStatus.Delete;
            return RedirectToAction(nameof(Index));
        }

    }
}