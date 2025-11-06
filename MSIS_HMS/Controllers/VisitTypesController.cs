using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Enums;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Infrastructure.Repositories;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Interfaces;
using MSIS_HMS.Models;
using X.PagedList;
using Microsoft.Extensions.Logging;

namespace MSIS_HMS.Controllers
{
    public class VisitTypesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IVisitTypeRepository _visitTypeRepository;
        private readonly IBranchService _branchService;
        private readonly IUserService _userService;
        private readonly Pagination _pagination;
        private readonly ILogger<VisitTypesController> _logger;

        public VisitTypesController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IVisitTypeRepository visitTypeRepository, IBranchService branchService, IUserService userService, IOptions<Pagination> pagination, ILogger<VisitTypesController> logger)
        {
            _userManager = userManager;
            _context = context;
            _visitTypeRepository = visitTypeRepository;
            _branchService = branchService;
            _userService = userService;
            _pagination = pagination.Value;
            _logger = logger;
        }

        public void Initialize(VisitType visitType = null)
        {
        }

        // GET
        public IActionResult Index(string VisitTypeName = null, int? page = 1)
        {

            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            var visitTypes = _visitTypeRepository.GetAll(_userService.Get(User).BranchId).Where(itemtype => string.IsNullOrEmpty(VisitTypeName) || (itemtype.Type.ToLower().Contains(VisitTypeName.ToLower())) || VisitTypeName.ToLower().Contains(itemtype.Type.ToLower())).ToList();
            var branches = _branchService.GetAll();
            visitTypes.ForEach(x => x.Branch = branches.SingleOrDefault(b => b.Id == x.BranchId));
            return View(visitTypes.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }

        public IActionResult Create()
        {
            Initialize();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VisitType visitType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    visitType = await _userManager.AddUserAndTimestamp(visitType, User, DbEnum.DbActionEnum.Create);
                    var _visitType = await _visitTypeRepository.AddAsync(visitType);
                    if (_visitType != null)
                    {
                        TempData["notice"] = StatusEnum.NoticeStatus.Success;
                        _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }
            Initialize(visitType);
            return View();
        }

        public IActionResult Edit(int id)
        {
            var visitType = _visitTypeRepository.Get(id);
            Initialize(visitType);
            return View(visitType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VisitType visitType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    visitType = await _userManager.AddUserAndTimestamp(visitType, User, DbEnum.DbActionEnum.Update);
                    var _visitType = await _visitTypeRepository.UpdateAsync(visitType);
                    TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                    _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }
            Initialize(visitType);
            return View(visitType);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var _visitType = await _visitTypeRepository.DeleteAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Delete));
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }

            return RedirectToAction(nameof(Index));
        }

    }
}