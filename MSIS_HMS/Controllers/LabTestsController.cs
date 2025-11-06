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
    public class LabTestsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILabTestRepository _labTestRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IBranchService _branchService;
        private readonly IUserService _userService;
        private readonly Pagination _pagination;
        private readonly ILogger<LabTestsController> _logger;

        public LabTestsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILabTestRepository labTestRepository, IDepartmentRepository departmentRepository, IDoctorRepository doctorRepository, IBranchService branchService, IUserService userService, IOptions<Pagination> pagination, ILogger<LabTestsController> logger)
        {
            _userManager = userManager;
            _context = context;
            _labTestRepository = labTestRepository;
            _doctorRepository = doctorRepository;
            _departmentRepository = departmentRepository;
            _branchService = branchService;
            _userService = userService;
            _pagination = pagination.Value;
            _logger = logger;
        }

        public void Initialize(LabTest labTest = null)
        {

        }

        // GET
        public IActionResult Index(string Name = null, string Code = null, LabCategoryEnum? Category = null, bool? IsLabReport = null, int? page = 1)
        {

            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            var labTests = _labTestRepository.GetAll(_userService.Get(User).BranchId, null, Name, Code, Category, IsLabReport).ToList();
            return View(labTests.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }

        public IActionResult Create()
        {
            Initialize();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LabTest labTest)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if(labTest.Category==LabCategoryEnum.Radiology)
                    {
                        labTest.IsLabReport = true;
                    }
                    labTest = await _userManager.AddUserAndTimestamp(labTest, User, DbEnum.DbActionEnum.Create);
                    var _labTest = await _labTestRepository.AddAsync(labTest);
                    if (_labTest != null)
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
            return View(labTest);
        }

        public ActionResult Edit(int id)
        {
            Initialize();
            var _labTests = _labTestRepository.Get(id);
            return View(_labTests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(LabTest labTest)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    labTest = await _userManager.AddUserAndTimestamp(labTest, User, DbEnum.DbActionEnum.Update);
                    var _labTest = await _labTestRepository.UpdateAsync(labTest);
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
            return View(labTest);
        }
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var _labTest = await _labTestRepository.DeleteAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Delete));
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult GetAll(bool? isImaging)
        {
            if(isImaging==true)
            {
                var labTests = _labTestRepository.GetAll(_userService.Get(User).BranchId).Where(x=>x.IsLabReport==true);
                return Ok(labTests);
            }
            else
            {
                var labTests = _labTestRepository.GetAll(_userService.Get(User).BranchId).Where(x=>x.IsLabReport==false);
                return Ok(labTests);
            }

        }
        public IActionResult GetLabTestByOrderId(int labOrderId)
        {
            var labTests = _labTestRepository.GetLabTestByOrderId(labOrderId);
            return Ok(labTests);
        }
        public IActionResult GetLabTestDetails(int LabTestId)
        {
            var labTestDetails = _labTestRepository.GetLabTestDetails(LabTestId);
            return Ok(labTestDetails);
        }
        public IActionResult GetLabResultDetails(int LabTestId)
        {
            var labTestDetails = _labTestRepository.GetLabTestDetails(LabTestId);
            var labResultDetails = labTestDetails.Select(x => new LabResultDetail
            {
                IsPrinted = true,
                LabUnit = x.LabUnit,
                MinRange = x.MinRange,
                MaxRange = x.MaxRange,
                Name = x.Name,
                IsTitle = x.IsTitle,
                LabResultType = x.LabResultType,
                SelectList = x.SelectList,
                SortOrder = x.SortOrder
            });
            return Ok(labResultDetails);
        }
    }

}