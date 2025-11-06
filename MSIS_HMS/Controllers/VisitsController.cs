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
using MSIS_HMS.Core.Entities.DTOs;

namespace MSIS_HMS.Controllers
{
    public class VisitsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IVisitRepository _visitRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly ISpecialityRepository _specialityRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IVisitTypeRepository _visitTypeRepository;
        private readonly IBranchService _branchService;
        private readonly IUserService _userService;
        private readonly Pagination _pagination;
        private readonly ILogger<VisitsController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VisitsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IVisitRepository visitRepository, ISpecialityRepository specialityRepository, IDepartmentRepository departmentRepository, IVisitTypeRepository visitTypeRepository, IPatientRepository patientRepository, IDoctorRepository doctorRepository, IBranchService branchService, IUserService userService, IOptions<Pagination> pagination, ILogger<VisitsController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _context = context;
            _visitRepository = visitRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _visitTypeRepository = visitTypeRepository;
            _specialityRepository = specialityRepository;
            _departmentRepository = departmentRepository;
            _branchService = branchService;
            _userService = userService;
            _pagination = pagination.Value;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public void Initialize(Visit visit = null, int? specialityId = null)
        {
            ViewData["Patients"] = _patientRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", "RegNo", visit?.PatientId);
            ViewData["Doctors"] = _doctorRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", visit?.DoctorId);
            ViewData["VisitTypes"] = _visitTypeRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Type", visit?.VisitTypeId);
            ViewData["Specialities"] = _specialityRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name");
            ViewData["Departments"] = _departmentRepository.GetAll(_userService.Get(User).BranchId).Where(x => x.Type == DepartmentTypeEnum.OPD).ToList().GetSelectListItems("Id", "Name");
        }

        // GET
        public IActionResult Index(string VisitNo = null, DateTime? StartDate = null, DateTime? EndDate = null, int? PatientId = null, int? DoctorId = null, int? VisitTypeId = null, VisitStatusEnum? Status = null, int? page = 1)
        {

            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            var visits = _visitRepository.GetAll(_userService.Get(User).BranchId, null, VisitNo, null, StartDate, EndDate, PatientId, DoctorId, VisitTypeId, Status).ToList();
            Visit visit = new Visit
            {
                PatientId = PatientId ?? 0,
                DoctorId = DoctorId ?? 0,
                VisitTypeId = VisitTypeId ?? 0
            };
            Initialize(visit);
            return View(visits.OrderBy(x => x.Status).ThenByDescending(x => x.Date).ToList().ToPagedList((int)page, pageSize));
        }
        public IActionResult VisitReport(string VisitNo = null, DateTime? FromDate = null, DateTime? ToDate = null, int? PatientId = null, int? DoctorId = null, int? VisitTypeId = null, VisitStatusEnum? Status = null, int? page = 1)
        {

            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["Patients"] = _patientRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name");
            ViewData["Doctors"] = _doctorRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name");
            if (FromDate==null)
            {
                FromDate = DateTime.Now.Date;
                ToDate = DateTime.Now.Date;
            }
            var visits = _visitRepository.GetAll(_userService.Get(User).BranchId, null, VisitNo, null, FromDate, ToDate, PatientId, DoctorId, VisitTypeId, Status).ToList();
            Visit visit = new Visit
            {
                PatientId = PatientId ?? 0,
                DoctorId = DoctorId ?? 0,
                VisitTypeId = VisitTypeId ?? 0
            };
            TempData["Page"] = page;
            TempData["StartDate"] = FromDate;
            TempData["EndDate"] = ToDate;
            TempData["Status"] = Status;
            TempData["PatientId"] = PatientId;
            TempData["DoctorId"] = DoctorId;
            Initialize(visit);
            return View(visits.OrderBy(x => x.Status).ThenByDescending(x => x.Date).ToList().ToPagedList((int)page, pageSize));
        }
        public IActionResult Create(int? patientId = null, int? specialityId = null)
        {
            Initialize();
            var visit = new Visit
            {
                VisitNo = _branchService.GetVoucherNo(VoucherTypeEnum.Visit),
                Status = VisitStatusEnum.Booked,
            };
            if (patientId != null)
            {
                visit.PatientId = (int)patientId;
            }
            return View(visit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Visit visit, int? specialityId = null)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var branch = _branchService.GetBranchByUser();
                        visit.VisitNo = _branchService.GetVoucherNo(VoucherTypeEnum.Visit, visit.VisitNo);
                        visit.CFFee =Convert.ToDecimal(_doctorRepository.Get(visit.DoctorId).CFFee);
                        if (string.IsNullOrEmpty(visit.VisitNo))
                        {
                            ModelState.AddModelError("VisitNo", "This field is required.");
                            Initialize(visit, specialityId);
                            return View(visit);
                        }
                        visit = await _userManager.AddUserAndTimestamp(visit, User, DbEnum.DbActionEnum.Create);
                        var _visit = await _visitRepository.AddAsync(visit);
                        if (_visit != null)
                        {
                            await _branchService.IncreaseVoucherNo(VoucherTypeEnum.Visit);
                            await transaction.CommitAsync();
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

            }
            Initialize(visit, specialityId);
            return View();
        }

        public IActionResult Edit(int id, int? specialityId)
        {
            var visit = _visitRepository.Get(id);
            Initialize(visit, specialityId);
            return View(visit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Visit visit, int? specialityId = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if(visit.DoctorId != (_visitRepository.Get(visit.Id).DoctorId))
                    {
                        visit.CFFee =Convert.ToDecimal(_doctorRepository.Get(visit.DoctorId).CFFee);
                    }
                    visit = await _userManager.AddUserAndTimestamp(visit, User, DbEnum.DbActionEnum.Update);
                    var _visit = await _visitRepository.UpdateAsync(visit);
                    TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                    _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }
            Initialize(visit, specialityId);
            return View(visit);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var _visit = await _visitRepository.DeleteAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Delete));
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult VisitDetail(int id, int? specialityId)
        {
            var visit = _visitRepository.Get(id);
            Initialize(visit, specialityId);
            return View(visit);
        }
        public async Task<IActionResult> IsComplete(int completeStatus , int id, VisitStatusEnum status)
        {
            var visit = await _context.Visits.FindAsync(id);
            visit.Completed = completeStatus;
            visit.Status = status;
            await _context.SaveChangesAsync();
            return Ok();            
        }
        public IActionResult GetVisitById(int id)
        {
            var visit = _visitRepository.Get(id);
            //Initialize(visit);
            return Ok(visit);
        }
        public IActionResult PrintReceipt(int id)
        {

            string renderFormat = "PDF";
            string extension = "pdf";
            string mimetype = "application/pdf";
            using (var report = new LocalReport())
            {
                List<Visit> visits = new List<Visit>();
                List<Branch> branches = new List<Branch>();
                var visit = _visitRepository.Get(id);
                var branch = _branchService.GetBranchById(visit.BranchId);
                visits.Add(visit);
                branches.Add(branch);

                report.DataSources.Add(new ReportDataSource("Visit", visits));
                report.DataSources.Add(new ReportDataSource("Branch", branches));
                report.ReportPath = $"{_webHostEnvironment.WebRootPath}\\ReportFiles\\VisitReceipt80mm.rdlc";

                var pdf = report.Render(renderFormat);
                return File(pdf, mimetype, visit.PatientName+"_Visit_"+DateTime.Now+"." + extension);
            }

        }

        public IActionResult DownloadReport()
        {
            var page = TempData["Page"];

            DateTime? fromDate = Convert.ToDateTime(TempData["StartDate"]);
            DateTime? toDate = Convert.ToDateTime(TempData["EndDate"]);
            int? PatientId = null;
            int? DoctorId = null;
            VisitStatusEnum? status = null;
            if (TempData["Status"] == null)
            {
                status = (VisitStatusEnum?)TempData["Status"];
            }
            else
            {
                status = (VisitStatusEnum)TempData["Status"];
            }
           
            if(TempData["PatientId"]!=null)
            {
                PatientId = (int)TempData["PatientId"];
            }
            if(TempData["DoctorId"]!=null)
            {
                DoctorId = (int)TempData["DoctorId"];
            }

            string renderFormat = "PDF";
            string extension = "pdf";
            string mimetype = "application/pdf";
            using (var report = new LocalReport())
            {
                List<Visit> visits = new List<Visit>();
                List<Branch> branches = new List<Branch>();
                var user = _userService.Get(User);
                 visits = _visitRepository.GetAll(_userService.Get(User).BranchId, null, null, null, fromDate, toDate, PatientId, DoctorId, null,status).ToList();
                foreach(var v in visits)
                {
                    if (v.Status == VisitStatusEnum.Booked)
                        v.StatusDesc = "Booked";
                    else if (v.Status == VisitStatusEnum.Completed)
                        v.StatusDesc = "Completed";
                    else
                        v.StatusDesc = "Cancel";
                }
                var branch = _branchService.GetBranchById((int)user.BranchId);
                branches.Add(branch);
                report.DataSources.Add(new ReportDataSource("Branch", branches));
                report.DataSources.Add(new ReportDataSource("dsVisit", visits));
                report.ReportPath = $"{_webHostEnvironment.WebRootPath}\\ReportFiles\\VisitReport.rdlc";

                var pdf = report.Render(renderFormat);
                TempData["Page"] = page;
                TempData["StartDate"] = fromDate;
                TempData["EndDate"] = toDate;
                TempData["Status"] = status;
                TempData["PatientId"] = PatientId;
                TempData["DoctorId"] = DoctorId;
                return File(pdf, mimetype,  "VisitReport_" + DateTime.Now + "." + extension);
            }
        }

        public IActionResult CFFeeReport(int? page = 1, DateTime? FromDate = null, DateTime? ToDate = null,int? DoctorId=null)
        {

            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["Patients"] = _patientRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name");
            ViewData["Doctors"] = _doctorRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name");
            if (FromDate == null)
            {
                FromDate = DateTime.Now.Date;
                ToDate = DateTime.Now.Date;
            }
            var visits = _visitRepository.GetCFFeeReport(_userService.Get(User).BranchId, FromDate, ToDate, DoctorId).ToList();
        
            TempData["Page"] = page;
            TempData["StartDate"] = FromDate;
            TempData["EndDate"] = ToDate;
            TempData["Doctor"] = DoctorId;
           
            //Initialize(visit);
            return View(visits.ToList().ToPagedList((int)page, pageSize));
          
        }
        public IActionResult DownloadCFFeeReport()
        {
            var page = TempData["Page"];

            DateTime? fromDate = Convert.ToDateTime(TempData["StartDate"]);
            DateTime? toDate = Convert.ToDateTime(TempData["EndDate"]);
            int? DoctorId = null;           
            
            if (TempData["DoctorId"] != null)
            {
                DoctorId = (int)TempData["DoctorId"];
            }

            string renderFormat = "PDF";
            string extension = "pdf";
            string mimetype = "application/pdf";
            using (var report = new LocalReport())
            {
                List<CFFeeReportDTO> visits = new List<CFFeeReportDTO>();
                List<Branch> branches = new List<Branch>();
                var user = _userService.Get(User);
                visits = _visitRepository.GetCFFeeReport(_userService.Get(User).BranchId, fromDate, toDate, DoctorId).ToList();
                
                var branch = _branchService.GetBranchById((int)_userService.Get(User).BranchId);
                branches.Add(branch);
                ReportParameter[] parameters = new ReportParameter[2];
                parameters[0] = new ReportParameter("FromDate", fromDate.ToString());
                parameters[1] = new ReportParameter("ToDate", toDate.ToString());

                report.DataSources.Add(new ReportDataSource("Branch", branches));
                report.DataSources.Add(new ReportDataSource("CFFeeReportDataSet", visits));
                report.ReportPath = $"{_webHostEnvironment.WebRootPath}\\ReportFiles\\CFFeeReport.rdlc";
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                TempData["Page"] = page;
                TempData["StartDate"] = fromDate;
                TempData["EndDate"] = toDate;
                TempData["DoctorId"] = DoctorId;
                return File(pdf, mimetype, "CFFeeReport_" + DateTime.Now + "." + extension);
            }
        }
        public IActionResult GetDataForPrintSlip(int id)
        {
            var visit = _visitRepository.Get(id);
            var branch = _branchService.GetBranchByUser();

            Slip slip = new Slip();
            slip.Hospital = branch.Name;
            slip.Address = branch.Address;
            slip.Phone = branch.Phone;
            slip.Doctor = visit.DoctorName;
            slip.Patient = visit.PatientName;
            slip.VoucherNo = visit.VisitNo;
            slip.Date = DateTime.Now.ToString("dd-MM-yyyy hh:mmtt");
            slip.SubTotal = string.Format("{0:#,###}", visit.CFFee);
            slip.Tax = string.Format("{0:#,###}", "0");
            slip.Discount = string.Format("{0:#,###}", "0");
            slip.GrandTotal = string.Format("{0:#,###}", visit.CFFee);
            SlipItem item = new SlipItem("CF Fee","", string.Format("{0:#,###}", visit.CFFee));
            slip.SlipItems = new List<SlipItem>
            {
                item
            };
            return Ok(slip);
        }
    }

}