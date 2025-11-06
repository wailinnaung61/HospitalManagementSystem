using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Reporting.NETCore;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Enums;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Interfaces;
using MSIS_HMS.Models;
using NLog;
using X.PagedList;

namespace MSIS_HMS.Controllers
{
    public class ServicesController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IServiceTypeRepository _serviceTypeRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IUserService _userService;
        private readonly IOutletRepository _outletRepository;
        private readonly IBranchService _branchService;
        private readonly ILogger<ServicesController> _logger;
        private readonly Pagination _pagination;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ServicesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IUserService userService, IDepartmentRepository departmentRepository, IServiceTypeRepository serviceTypeRepository, IServiceRepository serviceRepository,ILogger<ServicesController> logger,IOutletRepository outletRepository,IBranchService branchService, IOptions<Pagination> pagination,IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _departmentRepository = departmentRepository;
            _serviceTypeRepository = serviceTypeRepository;
            _serviceRepository = serviceRepository;
            _userService = userService;
            _logger = logger;
            _outletRepository = outletRepository;
            _branchService = branchService;
            _pagination = pagination.Value;
            _webHostEnvironment = webHostEnvironment;
        }

        public void Initialize()
        {
            var serviceTypes = _serviceTypeRepository.GetAll(_userService.Get(User).BranchId);
            ViewData["ServiceTypes"] = new SelectList(serviceTypes, "Id", "Name");
        }

        public IActionResult Index()
        {
            var service = _serviceRepository.GetAll();
            foreach (var obj in service)
            {
                obj.ServiceType = _context.ServiceTypes.FirstOrDefault(u => u.Id == obj.ServiceTypeId);
            }
            return View(service);
        }
        public IActionResult DailyServiceReport(int? page = 1, DateTime? FromDate = null, DateTime? ToDate = null, int? OutletId = null)
        {
            if (FromDate == null)
            {
                FromDate = DateTime.Now.Date;
                ToDate = DateTime.Now.Date;
            }
            var outlets = _outletRepository.GetAll(_branchService.GetBranchIdByUser());
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["Outlets"] = new SelectList(outlets, "Id", "Name");
            var items = _serviceRepository.GetServiceFromOrder(_userService.Get(User).BranchId, FromDate, ToDate, OutletId);
            TempData["Page"] = page;
            TempData["FromDate"] = FromDate;
            TempData["ToDate"] = ToDate;
            TempData["OutletId"] = OutletId;
            return View(items.ToList().ToPagedList((int)page, pageSize));
        }
        public ActionResult Create()
        {
            Initialize();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service service)
        {
            MappedDiagnosticsLogicalContext.Set("userId", _userManager.GetUserId(User));
            try
            {
                if (ModelState.IsValid)
                {
                    service = await _userManager.AddUserAndTimestamp(service, User, DbEnum.DbActionEnum.Create);
                    var _service = await _serviceRepository.AddAsync(service);
                    if (_service != null)
                    {
                        TempData["notice"] = StatusEnum.NoticeStatus.Success;
                        _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _logger.LogError(e.InnerException.Message);
            }
            Initialize();
            return View();
        }

        public ActionResult Edit(int id)
        {
            Initialize();
            var _service = _serviceRepository.Get(id);
            return View(_service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Service service)
        {
            MappedDiagnosticsLogicalContext.Set("userId", _userManager.GetUserId(User));
            try
            {
                if (ModelState.IsValid)
                {
                    service = await _userManager.AddUserAndTimestamp(service, User, DbEnum.DbActionEnum.Update);
                    var _service = await _serviceRepository.UpdateAsync(service);
                    TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                    _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _logger.LogError(e.InnerException.Message);
            }
            Initialize();
            return View(service);
        }

        public async Task<ActionResult> Delete(int id)
        {
            MappedDiagnosticsLogicalContext.Set("userId", _userManager.GetUserId(User));
            try
            {
                var _service = await _serviceRepository.DeleteAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Delete));
            }
            catch(Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }
           
            return RedirectToAction(nameof(Index));
        }

        public IActionResult GetAll()
        {
            var units = _serviceRepository.GetAll(_userService.Get(User).BranchId);
            return Ok(units.OrderBy(x => x.Name).ToList());
        }
        public IActionResult DownloadReport()
        {
            var page = TempData["Page"];

            DateTime? fromDate = Convert.ToDateTime(TempData["FromDate"]);
            DateTime? toDate = Convert.ToDateTime(TempData["ToDate"]);
            int? outletId = null;
            if (TempData["FromDate"] == null)
            {
                fromDate = DateTime.Now.Date;
                toDate = DateTime.Now.Date;

            }
            if (TempData["OutletId"] != null)
            {
                outletId = (int)TempData["OutletId"];
            }
            string renderFormat = "PDF";
            string extension = "pdf";
            string mimetype = "application/pdf";
            using (var report = new LocalReport())
            {
                List<ServiceDTO> Orders = new List<ServiceDTO>();
                List<Branch> branches = new List<Branch>();

                var user = _userService.Get(User);
                Orders = _serviceRepository.GetServiceFromOrder(_userService.Get(User).BranchId, fromDate, toDate, outletId);

                var branch = _branchService.GetBranchById((int)user.BranchId);
                branches.Add(branch);
                ReportParameter[] parameters = new ReportParameter[2];
                parameters[0] = new ReportParameter("FromDate", fromDate.ToString());
                parameters[1] = new ReportParameter("ToDate", toDate.ToString());

                report.DataSources.Add(new ReportDataSource("Branch", branches));
                report.DataSources.Add(new ReportDataSource("dsService", Orders));

                report.ReportPath = $"{_webHostEnvironment.WebRootPath}\\ReportFiles\\ServiceReport.rdlc";
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat); TempData["Page"] = page;
                TempData["FromDate"] = fromDate;
                TempData["ToDate"] = toDate;
                TempData["OutletId"] = outletId;
                return File(pdf, mimetype, "DailyServiceReport_" + DateTime.Now + "." + extension);
            }
        }
    }
}