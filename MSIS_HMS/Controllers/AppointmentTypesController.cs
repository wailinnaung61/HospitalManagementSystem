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
    public class AppointmentTypesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IAppointmentTypeRepository _appointmentTypeRepository;
        private readonly IBranchService _branchService;
        private readonly IUserService _userService;
        private readonly Pagination _pagination;
        private readonly ILogger<AppointmentTypesController> _logger;

        public AppointmentTypesController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IAppointmentTypeRepository appointmentTypeRepository, IBranchService branchService, IUserService userService, IOptions<Pagination> pagination, ILogger<AppointmentTypesController> logger)
        {
            _userManager = userManager;
            _context = context;
            _appointmentTypeRepository = appointmentTypeRepository;
            _branchService = branchService;
            _userService = userService;
            _pagination = pagination.Value;
            _logger = logger;
        }

        public void Initialize(AppointmentType appointmentType = null)
        {
        }

        // GET
        public IActionResult Index(string AppointmentTypeName = null, int? page = 1)
        {

            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            var appointmentTypes = _appointmentTypeRepository.GetAll(_userService.Get(User).BranchId).Where(itemtype => string.IsNullOrEmpty(AppointmentTypeName) || (itemtype.Type.ToLower().Contains(AppointmentTypeName.ToLower())) || AppointmentTypeName.ToLower().Contains(itemtype.Type.ToLower())).ToList();
            var branches = _branchService.GetAll();
            appointmentTypes.ForEach(x => x.Branch = branches.SingleOrDefault(b => b.Id == x.BranchId));
            return View(appointmentTypes.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }

        public IActionResult Create()
        {
            Initialize();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentType appointmentType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    appointmentType = await _userManager.AddUserAndTimestamp(appointmentType, User, DbEnum.DbActionEnum.Create);
                    var _appointmentType = await _appointmentTypeRepository.AddAsync(appointmentType);
                    if (_appointmentType != null)
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
            Initialize(appointmentType);
            return View();
        }

        public IActionResult Edit(int id)
        {
            var appointmentType = _appointmentTypeRepository.Get(id);
            Initialize(appointmentType);
            return View(appointmentType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppointmentType appointmentType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    appointmentType = await _userManager.AddUserAndTimestamp(appointmentType, User, DbEnum.DbActionEnum.Update);
                    var _appointmentType = await _appointmentTypeRepository.UpdateAsync(appointmentType);
                    TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                    _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }
            Initialize(appointmentType);
            return View(appointmentType);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var _appointmentType = await _appointmentTypeRepository.DeleteAsync(id);
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