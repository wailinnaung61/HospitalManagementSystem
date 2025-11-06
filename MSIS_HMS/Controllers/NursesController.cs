using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Core.Repositories.Base;
using MSIS_HMS.Enums;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Models;
using X.PagedList;


namespace MSIS_HMS.Controllers
{
    public class NursesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INurseRepository _nurseRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ApplicationDbContext _context;
        private readonly Pagination _pagination;
        private readonly ILogger<NursesController> _logger;
        public NursesController(IDepartmentRepository departmentRepository, INurseRepository nurseRepository, ApplicationDbContext context, UserManager<ApplicationUser> userManager, IOptions<Pagination> pagination,ILogger<NursesController> logger)
        {
            _departmentRepository = departmentRepository;
            _nurseRepository = nurseRepository;
            _context = context;
            _userManager = userManager;
            _pagination = pagination.Value;
            _logger = logger;
        }
        public void Initialize()
        {
            var departments = _departmentRepository.GetAll();
            ViewData["Departments"] = new SelectList(departments, "Id", "Name");
        }
        public IActionResult Create()
        {
            Initialize();
            return View();
        }

        // GET
        public IActionResult Index(int? page = 1, string NurseName = null, string Code = null, int? DepartmentId = null)
        {
            Initialize();
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;

            var nurses = _nurseRepository.GetAll().Where(nurse =>
                (DepartmentId == null || nurse.DepartmentId == DepartmentId) &&
                (NurseName == null || nurse.Name.ToLower().Contains(NurseName.ToLower()) || NurseName.ToLower().Contains(nurse.Name.ToLower())) &&
                (Code == null || nurse.Code.ToLower().Contains(Code.ToLower()) || Code.ToLower().Contains(nurse.Code.ToLower()))).ToList();

            foreach (var obj in nurses)
            {
                obj.Department = _context.Departments.FirstOrDefault(u => u.Id == obj.DepartmentId);
            }
            return View(nurses.OrderByDescending(nurse => nurse.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Nurse nurse)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    nurse = await _userManager.AddUserAndTimestamp(nurse, User, DbEnum.DbActionEnum.Create);
                    var _nurse = await _nurseRepository.AddAsync(nurse);
                    if (_nurse != null)
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
            Initialize();
            return View();
        }

        public ActionResult Edit(int id)
        {
            Initialize();
            var _nurses = _nurseRepository.Get(id);
            return View(_nurses);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Nurse nurse)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    nurse = await _userManager.AddUserAndTimestamp(nurse, User, DbEnum.DbActionEnum.Update);
                    var _nurse = await _nurseRepository.UpdateAsync(nurse);
                    TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                    _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }
            Initialize();
            return View(nurse);
        }
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var _nurse = await _nurseRepository.DeleteAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Delete));
            }
            catch(Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }           
            return RedirectToAction(nameof(Index));
        }
    }
}