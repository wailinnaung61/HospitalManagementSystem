using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Enums;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Infrastructure.Repositories;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Interfaces;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MSIS_HMS.Models;
using Microsoft.Extensions.Options;
using X.PagedList;
using static MSIS_HMS.Infrastructure.Enums.DbEnum;
using Microsoft.Reporting.NETCore;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace MSIS_HMS.Controllers
{
    public class LabOrdersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILabOrderRepository _labOrderRepository;
        private readonly IUserService _userService;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IOutletRepository _outletRepository;
        private readonly IBranchService _branchService;
        private readonly ILogger<LabOrdersController> _logger;
        private readonly ILabTestRepository _labTestRepository;
        private readonly Pagination _pagination;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LabOrdersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IBranchService branchService, ILabOrderRepository labOrderRepository, IUserService userService, ILogger<LabOrdersController> logger, IOptions<Pagination> pagination, IDoctorRepository doctorRepository, IPatientRepository patientRepository, IOutletRepository outletRepository, IWebHostEnvironment webHostEnvironment, ILabTestRepository labTestRepository)
        {
            _userManager = userManager;
            _context = context;
            _labOrderRepository = labOrderRepository;
            _userService = userService;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _outletRepository = outletRepository;
            _branchService = branchService;
            _logger = logger;
            _pagination = pagination.Value;
            _webHostEnvironment = webHostEnvironment;
            _labTestRepository = labTestRepository;
        }

        public void Initialize(LabOrder labOrder = null)
        {
            ViewData["Patients"] = _patientRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", "RegNo", labOrder?.PatientId);
            var branch = _branchService.GetBranchByUser();
            ViewData["UseVoucherFormat"] = branch.UseVoucherFormatForOrder;
        }

        // GET
        public IActionResult Index(int? page = 1, int? OrderId = null, int? PatientId = null, int? DoctorId = null, string VoucherNo = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            var user = _userService.Get(User);
            var labOrders = _labOrderRepository.GetAll(user.BranchId, null, PatientId, VoucherNo, null, null, StartDate, EndDate, null, null, null);
            ViewData["Patients"] = _patientRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", "RegNo", PatientId);
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            return View(labOrders.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }

        public IActionResult LabOrderReport(int? page = 1, int? OrderId = null,int? PatientId=null, int? DoctorId = null, DateTime? FromDate = null, DateTime? ToDate = null,int? TestId=null)
        {
            var user = _userService.Get(User);
            ViewData["Patients"] = _patientRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name");
            ViewData["Tests"] = _labTestRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name");
            if (FromDate==null)
            {
                FromDate = DateTime.Now.Date;
                ToDate = DateTime.Now.Date;
            }

            var labOrders = _labOrderRepository.GetAll(user.BranchId, null, PatientId, null, true, null, FromDate, ToDate, null, null, null,TestId);
            //ViewData["Patients"] = _patientRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", "RegNo", PatientId);
            foreach (var labO in labOrders)
            {
                var lTest = _labTestRepository.GetLabTestByLabOrderId(labO.Id);
                for (int i = 0; i < lTest.Count; i++)
                {
                    if (lTest.Count == 1)
                        labO.LabTest = lTest[i].LabTestName;
                    else if (i != lTest.Count - 1)
                        labO.LabTest += lTest[i].LabTestName + ",";
                    else
                        labO.LabTest += lTest[i].LabTestName;

                }

            }
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;

            TempData["Page"] = page;
            TempData["FromDate"] = FromDate;
            TempData["ToDate"] = ToDate;
            TempData["PatientId"] = PatientId;
            TempData["TestId"] = TestId;

            return View(labOrders.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }


        public IActionResult Create()
        {
            Initialize();
            var labOrder = new LabOrder
            {
                VoucherNo = _branchService.GetVoucherNo(VoucherTypeEnum.Lab)
            };
            return View(labOrder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LabOrder labOrder)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (labOrder.LabOrderTests != null && labOrder.LabOrderTests.Count() > 0)
                        {
                            var branch = _branchService.GetBranchByUser();
                            labOrder.VoucherNo = _branchService.GetVoucherNo(VoucherTypeEnum.Lab, labOrder.VoucherNo);
                            if (string.IsNullOrEmpty(labOrder.VoucherNo))
                            {
                                ModelState.AddModelError("VoucherNo", "This field is required.");
                                Initialize(labOrder);
                                return View(labOrder);
                            }
                            if (branch.AutoPaidForOrder)
                            {
                                labOrder.IsPaid = true;
                                labOrder.PaidDate = DateTime.Now;
                            }
                            labOrder.Total = labOrder.LabOrderTests.CalculateTotal() + labOrder.Tax - labOrder.Discount; //_labOrderRepository.CalculateTotal(labOrder.OrderItems.ToList());
                            labOrder = await _userManager.AddUserAndTimestamp(labOrder, User, DbEnum.DbActionEnum.Create);
                            var _labOrder = await _labOrderRepository.AddAsync(labOrder);
                            if (_labOrder != null)
                            {
                                await _branchService.IncreaseVoucherNo(VoucherTypeEnum.Lab);
                                await transaction.CommitAsync();
                                TempData["notice"] = StatusEnum.NoticeStatus.Success;
                                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));
                                return RedirectToAction(nameof(Index));
                            }
                            throw new Exception();
                        }
                        else
                        {
                            ViewData["Error"] = "Order Items are required";
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    await transaction.RollbackAsync();
                }
            }
            Initialize(labOrder);
            return View(labOrder);
        }

        public IActionResult Edit(int id)
        {
            var labOrder = _labOrderRepository.Get(id);
            Initialize(labOrder);
            return View(labOrder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LabOrder labOrder)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (labOrder.LabOrderTests != null && labOrder.LabOrderTests.Count() > 0)
                        {
                            labOrder.Total = labOrder.LabOrderTests.CalculateTotal() + labOrder.Tax - labOrder.Discount; //_labOrderRepository.CalculateTotal(labOrder.OrderItems.ToList());
                            labOrder = await _userManager.AddUserAndTimestamp(labOrder, User, DbEnum.DbActionEnum.Update);
                            var _labOrder = await _labOrderRepository.UpdateAsync(labOrder);
                            if (_labOrder != null)
                            {
                                await transaction.CommitAsync();
                                TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {
                            ViewData["Error"] = "Tests are required";
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.InnerException.Message);
                    await transaction.RollbackAsync();
                }
            }
            Initialize(labOrder);
            return View(labOrder);
        }

        public async Task<IActionResult> Delete(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var labOrder = _labOrderRepository.Get(id);
                    var isSucceed = await _labOrderRepository.DeleteAsync(id);
                    if (isSucceed)
                    {
                        await transaction.CommitAsync();
                        TempData["notice"] = StatusEnum.NoticeStatus.Delete;
                        _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Delete));
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.InnerException.Message);
                    await transaction.RollbackAsync();
                    TempData["notice"] = StatusEnum.NoticeStatus.Fail;
                }
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Paid(int id)
        {
            var labOrder = await _context.LabOrders.FindAsync(id);
            if (labOrder != null)
            {
                labOrder.IsPaid = true;
                labOrder.PaidDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            else
            {
                TempData["notice"] = StatusEnum.NoticeStatus.Fail;
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult PrintReceipt(int id)
        {
            string renderFormat = "PDF";
            string extension = "pdf";
            string mimetype = "application/pdf";
            using (var report = new LocalReport())
            {
                var labOrder = _labOrderRepository.Get(id);
                var labOrderTests = labOrder.LabOrderTests.Select(x => new
                {
                    Item = x.LabTestName,
                    UnitPrice = x.UnitPrice.ToString("0.00"),
                    Qty = x.Qty,
                    Price = (x.UnitPrice * x.Qty).ToString("0.00"),
                    VoucherNo = labOrder.VoucherNo,
                    Date = labOrder.Date.ToString("dd-MM-yyyy"),
                    IsPaid = labOrder.IsPaid,
                    Tax = labOrder.Tax.ToString("0.00"),
                    Discount = labOrder.Discount.ToString("0.00"),
                    PatientName = labOrder.PatientName,
                    BranchName = labOrder.BranchName,
                    Address = labOrder.BranchAddress,
                    Phone = labOrder.BranchPhone
                }).ToList();
                report.DataSources.Add(new ReportDataSource("dsPharmacyReceipt", labOrderTests));
                report.ReportPath = $"{_webHostEnvironment.WebRootPath}\\ReportFiles\\LabOrderReceipt.rdlc";

                var pdf = report.Render(renderFormat);
                return File(pdf, mimetype, "report." + extension);
            }
        }

        public IActionResult PrintSlip(int id)
        {
            string renderFormat = "PDF";
            string extension = "pdf";
            string mimetype = "application/pdf";
            using (var report = new LocalReport())
            {
                var labOrder = _labOrderRepository.Get(id);
                var labOrderTests = labOrder.LabOrderTests.Select(x => new
                {
                    Item = x.LabTestName,
                    UnitPrice = x.UnitPrice.ToString("0.00"),
                    Qty = x.Qty,
                    Price = (x.UnitPrice * x.Qty).ToString("0.00"),
                    VoucherNo = labOrder.VoucherNo,
                    Date = labOrder.Date.ToString("dd-MM-yyyy"),
                    IsPaid = labOrder.IsPaid,
                    Tax = labOrder.Tax.ToString("0.00"),
                    Discount = labOrder.Discount.ToString("0.00"),
                    PatientName = labOrder.PatientName,
                    BranchName = labOrder.BranchName,
                    Address = labOrder.BranchAddress,
                    Phone = labOrder.BranchPhone,
                    UnitShortForm = ""


                }).ToList();
                report.DataSources.Add(new ReportDataSource("dsPharmacyReceipt", labOrderTests));
                report.ReportPath = $"{_webHostEnvironment.WebRootPath}\\ReportFiles\\PharmacyReceipt80mm.rdlc";

                var pdf = report.Render(renderFormat);
                return File(pdf, mimetype, "report." + extension);
            }
        }

        public IActionResult DownloadReport()
        {
            var page = TempData["Page"];

            DateTime? fromDate = Convert.ToDateTime(TempData["FromDate"]);
            DateTime? toDate = Convert.ToDateTime(TempData["ToDate"]);
            int? PatientId = null;
            int? TestId = null;
            if (TempData["FromDate"] == null)
            {
                fromDate = null;
                toDate = null;

            }
            if(TempData["PatientId"]!=null)
            {
                PatientId =(int) TempData["PatientId"];
            }
            if(TempData["TestId"]!=null)
            {
                TestId = (int)TempData["TestId"];
            }
            string renderFormat = "PDF";
            string extension = "pdf";
            string mimetype = "application/pdf";
            using (var report = new LocalReport())
            {
                List<LabOrder> labOrders = new List<LabOrder>();
                List<Branch> branches = new List<Branch>();
                var user = _userService.Get(User);
                labOrders = _labOrderRepository.GetAll(user.BranchId, null, PatientId, null, true, null, fromDate, toDate, null, null, null,TestId);
                foreach (var labO in labOrders)
                {
                    var lTest = _labTestRepository.GetLabTestByLabOrderId(labO.Id);
                    for (int i = 0; i < lTest.Count; i++)
                    {
                        if (lTest.Count == 1)
                            labO.LabTest = lTest[i].LabTestName;
                        else if (i != lTest.Count - 1)
                            labO.LabTest += lTest[i].LabTestName + ",";
                        else
                            labO.LabTest += lTest[i].LabTestName;

                    }

                }
                var branch = _branchService.GetBranchById((int)user.BranchId);
                branches.Add(branch);
                report.DataSources.Add(new ReportDataSource("Branch", branches));
                report.DataSources.Add(new ReportDataSource("dsLabOrder", labOrders));
                report.ReportPath = $"{_webHostEnvironment.WebRootPath}\\ReportFiles\\LabOrderReport.rdlc";

                var pdf = report.Render(renderFormat);
                TempData["Page"] = page;
                TempData["FromDate"] = fromDate;
                TempData["ToDate"] = toDate;
                TempData["PatientId"] = PatientId;
                TempData["TestId"] = TestId;
                return File(pdf, mimetype, "_LabOrderReport_" + DateTime.Now + "." + extension);
            }
        }

        public IActionResult GetLabOrderFromLabOrderTest()
        {
            var labOrders = _labOrderRepository.GetLabOrderFromLabOrderTest(_userService.Get(User).BranchId);
            return Ok(labOrders);
        }
    }
}