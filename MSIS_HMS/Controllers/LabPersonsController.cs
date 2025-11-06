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
using MSIS_HMS.Core.Enums;
using Microsoft.Reporting.NETCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using MSIS_HMS.Core.Repositories.Base;

namespace MSIS_HMS.Controllers
{
    public class LabPersonsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILabPersonRepository _labPersonRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IBranchService _branchService;
        private readonly IUserService _userService;
        private readonly Pagination _pagination;
        private readonly ILogger<LabPersonsController> _logger;

        public LabPersonsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILabPersonRepository labPersonRepository, IDepartmentRepository departmentRepository, IDoctorRepository doctorRepository, IBranchService branchService, IUserService userService, IOptions<Pagination> pagination, ILogger<LabPersonsController> logger)
        {
            _userManager = userManager;
            _context = context;
            _labPersonRepository = labPersonRepository;
            _doctorRepository = doctorRepository;
            _departmentRepository = departmentRepository;
            _branchService = branchService;
            _userService = userService;
            _pagination = pagination.Value;
            _logger = logger;
        }

        public void Initialize(LabPerson labPerson = null)
        {
            ViewData["Doctors"] = _doctorRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", labPerson?.DoctorId);
            ViewData["Departments"] = _departmentRepository.GetAll(_userService.Get(User).BranchId).ToList().GetSelectListItems("Id", "Name", labPerson?.DepartmentId);
        }

        // GET
        public IActionResult Index(string Name = null, string Code = null, int? DepartmentId = null, int? DoctorId = null, LabPersonTypeEnum? Type = null, int? page = 1)
        {

            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            var labPersons = _labPersonRepository.GetAll(_userService.Get(User).BranchId, null, Name, Code, Type, DepartmentId, DoctorId).ToList();
            ViewData["Departments"] = _departmentRepository.GetAll(_userService.Get(User).BranchId).ToList().GetSelectListItems("Id", "Name", DepartmentId);
            return View(labPersons.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }

        public IActionResult Create()
        {
            Initialize();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LabPerson labPerson, int? specialityId = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    labPerson = await _userManager.AddUserAndTimestamp(labPerson, User, DbEnum.DbActionEnum.Create);
                    var _labPerson = await _labPersonRepository.AddAsync(labPerson);
                    if (_labPerson != null)
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
            var _labPersons = _labPersonRepository.Get(id);
            return View(_labPersons);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(LabPerson labPerson)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    labPerson = await _userManager.AddUserAndTimestamp(labPerson, User, DbEnum.DbActionEnum.Update);
                    var _labPerson = await _labPersonRepository.UpdateAsync(labPerson);
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
            return View(labPerson);
        }
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var _labPerson = await _labPersonRepository.DeleteAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Delete));
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult GetAll(LabPersonTypeEnum? labPersonType)
        {
            var labPersons = _labPersonRepository.GetAll(_userService.Get(User).BranchId, null, null, null, labPersonType);
            return Ok(labPersons);
        }
        public IActionResult GetAllByLabTest(int LabTestId,bool? isImaging)
        {
            if(isImaging==false)
            {
                var labPersons = _labPersonRepository.GetLabPerson_LabTests(null, LabTestId).Select(x => new
                {
                    Id = x.LabPersonId,
                    Name = x.LabPersonName,
                    Type = x.LabPersonType,
                    IsLabReport = false
                }) ;
                return Ok(labPersons);
            }
            else
            {
                var labPersons = _labPersonRepository.GetLabPerson_LabTests(null, LabTestId).Select(x => new {
                    Id = x.LabPersonId,
                    Name = x.LabPersonName,
                    Type = x.LabPersonType,
                    IsLabReport = x.IsLabReport
                });

                return Ok(labPersons);
            }
            
        }
    }

}