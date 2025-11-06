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
using MSIS_HMS.Core.Enums;
using MSIS_HMS.Core.Entities.DTOs;

namespace MSIS_HMS.Controllers
{
    public class LabResultsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILabResultRepository _labResultRepository;
        private readonly ILabOrderRepository _labOrderRepository;
        private readonly ILabPersonRepository _labPersonRepository;
        private readonly ILabTestRepository _labTestRepository;
        private readonly IUserService _userService;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IOutletRepository _outletRepository;
        private readonly IBranchService _branchService;
        private readonly ILogger<LabResultsController> _logger;
        private readonly Pagination _pagination;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LabResultsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IBranchService branchService, ILabResultRepository labResultRepository, ILabPersonRepository labPersonRepository, ILabTestRepository labTestRepository, IUserService userService, ILogger<LabResultsController> logger, IOptions<Pagination> pagination, IDoctorRepository doctorRepository, IPatientRepository patientRepository, IOutletRepository outletRepository, IWebHostEnvironment webHostEnvironment, ILabOrderRepository labOrderRepository)
        {
            _userManager = userManager;
            _context = context;
            _labResultRepository = labResultRepository;
            _labPersonRepository = labPersonRepository;
            _labTestRepository = labTestRepository;
            _userService = userService;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _outletRepository = outletRepository;
            _branchService = branchService;
            _logger = logger;
            _pagination = pagination.Value;
            _labOrderRepository = labOrderRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public void Initialize(LabResult labResult = null)
        {
            ViewData["Patients"] = _patientRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", "RegNo", labResult?.PatientId);
            var labPersons = _labPersonRepository.GetAll(_userService.Get(User).BranchId);
            ViewData["Technicians"] = labPersons.Where(x => x.Type == LabPersonTypeEnum.Technician).ToList().GetSelectListItems("Id", "Name", labResult?.TechnicianId);
            ViewData["Consultants"] = labPersons.Where(x => x.Type == LabPersonTypeEnum.Consultant).ToList().GetSelectListItems("Id", "Name", labResult?.ConsultantId);
            var branch = _branchService.GetBranchByUser();
            ViewData["UseVoucherFormat"] = branch.UseVoucherFormatForOrder;
            ViewData["LabOrderNo"] = _labOrderRepository.GetLabOrderFromLabOrderTest(_userService.Get(User).BranchId).GetSelectListItems("Id", "VoucherNo");
        }

        // GET
        public IActionResult Index(int? page = 1, int? PatientId = null, string ResultNo = null, int? TechnicianId = null, int? ConsultantId = null, bool? IsCompleted = null, DateTime? StartDate = null, DateTime? EndDate = null, int? LabTestId = null)
        {
            var user = _userService.Get(User);
            var labResults = _labResultRepository.GetAll(user.BranchId, null, PatientId, ResultNo, TechnicianId, ConsultantId, IsCompleted, null, StartDate, EndDate);
            ViewData["Patients"] = _patientRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", "RegNo", PatientId);
            var labPersons = _labPersonRepository.GetAll(_userService.Get(User).BranchId);
            ViewData["Technicians"] = labPersons.Where(x => x.Type == LabPersonTypeEnum.Technician).ToList().GetSelectListItems("Id", "Name", TechnicianId);
            ViewData["Consultants"] = labPersons.Where(x => x.Type == LabPersonTypeEnum.Consultant).ToList().GetSelectListItems("Id", "Name", ConsultantId);
            ViewData["LabTests"] = _labTestRepository.GetAll(_userService.Get(User).BranchId).ToList().GetSelectListItems("Id", "Name", LabTestId);
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            return View(labResults.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }

        public IActionResult ServiceFees(int? page = 1, int? PatientId = null, string ResultNo = null, int? TechnicianId = null, int? ConsultantId = null, bool? IsCompleted = null, DateTime? StartDate = null, DateTime? EndDate = null, int? LabTestId = null)
        {
            var user = _userService.Get(User);
            var labResults = _labResultRepository.GetAll(user.BranchId, null, PatientId, ResultNo, TechnicianId, ConsultantId, true, null, null, null, null, StartDate, EndDate,LabTestId);
            ViewData["Patients"] = _patientRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", "RegNo", PatientId);
            var labPersons = _labPersonRepository.GetAll(_userService.Get(User).BranchId);
            ViewData["Technicians"] = labPersons.Where(x => x.Type == LabPersonTypeEnum.Technician).ToList().GetSelectListItems("Id", "Name", TechnicianId);
            ViewData["Consultants"] = labPersons.Where(x => x.Type == LabPersonTypeEnum.Consultant).ToList().GetSelectListItems("Id", "Name", ConsultantId);
            ViewData["LabTests"] = _labTestRepository.GetAll(_userService.Get(User).BranchId).ToList().GetSelectListItems("Id", "Name", LabTestId);
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            return View(labResults.Where(x => x.TechnicianId != null || x.ConsultantId != null).OrderByDescending(x => x.CompletedDate).ThenBy(x => x.ResultNo).ToList().ToPagedList((int)page, pageSize));
        }

        public IActionResult ServiceFeesReport(int? page = 1, int? PatientId = null, string ResultNo = null, int? TechnicianId = null, int? ConsultantId = null, bool? IsCompleted = null, DateTime? FromDate = null, DateTime? ToDate = null, int? LabTestId = null)
        {
            var user = _userService.Get(User);
            var labResults = _labResultRepository.GetAll(user.BranchId, null, PatientId, ResultNo, TechnicianId, ConsultantId, true,null, null, null,null , FromDate, ToDate,null);
            ViewData["Patients"] = _patientRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", "RegNo", PatientId);
            var labPersons = _labPersonRepository.GetAll(_userService.Get(User).BranchId);
            ViewData["Technicians"] = labPersons.Where(x => x.Type == LabPersonTypeEnum.Technician).ToList().GetSelectListItems("Id", "Name", TechnicianId);
            ViewData["Consultants"] = labPersons.Where(x => x.Type == LabPersonTypeEnum.Consultant).ToList().GetSelectListItems("Id", "Name", ConsultantId);
            ViewData["LabTests"] = _labTestRepository.GetAll(_userService.Get(User).BranchId).ToList().GetSelectListItems("Id", "Name", LabTestId);
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            TempData["FromDate"] = FromDate;
            TempData["ToDate"] = ToDate;
            TempData["ResultNo"] = ResultNo;
            TempData["TechnicianId"] = TechnicianId;
            TempData["ConsultantId"] = ConsultantId;
                
            return View(labResults.Where(x => x.TechnicianId != null || x.ConsultantId != null).OrderByDescending(x => x.CompletedDate).ThenBy(x => x.ResultNo).ToList().ToPagedList((int)page, pageSize));
        }

        public IActionResult Create()
        {
            Initialize();
            var labResult = new LabResult
            {
                ResultNo = _branchService.GetVoucherNo(VoucherTypeEnum.LabResult)
            };
            return View(labResult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LabResult labResult)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    ModelState.Remove("TechnicianFeeType");
                    ModelState.Remove("ConsultantFeeType");
                    ModelState.Remove("LabOrderId");
                    if (ModelState.IsValid)
                    {
                        if (labResult.LabResultDetails != null && labResult.LabResultDetails.Count() > 0)
                        {
                            var branch = _branchService.GetBranchByUser();
                            labResult.ResultNo = _branchService.GetVoucherNo(VoucherTypeEnum.LabResult, labResult.ResultNo);
                            labResult.UnitPrice = _labTestRepository.Get(labResult.LabTestId).UnitPrice;
                            if(labResult.TechnicianId != null)
                            {
                                var technicianFee = _labPersonRepository.GetLabPerson_LabTests(labResult.TechnicianId, labResult.LabTestId).First();
                                labResult.TechnicianFee = technicianFee.Fee;
                                labResult.TechnicianFeeType = technicianFee.FeeType;
                            }
                            if (labResult.ConsultantId != null)
                            {
                                var consultantFee = _labPersonRepository.GetLabPerson_LabTests(labResult.ConsultantId, labResult.LabTestId).First();
                                labResult.ConsultantFee = consultantFee.Fee;
                                labResult.ConsultantFeeType = consultantFee.FeeType;
                            }
                            foreach(var labResultDetail in labResult.LabResultDetails)
                            {
                                var consultantFee = _labPersonRepository.GetLabPerson_LabTests(labResultDetail.ConsultantId, labResultDetail.TestId).First();
                                labResultDetail.ConsultantFee = consultantFee.Fee;
                                labResultDetail.ConsultantFeeType = consultantFee.FeeType;
                                var technicianFee = _labPersonRepository.GetLabPerson_LabTests(labResultDetail.TechnicianId, labResultDetail.TestId).First();
                                labResultDetail.TechnicianFee = technicianFee.Fee;
                                labResultDetail.TechnicianFeeType = technicianFee.FeeType;

                            }
                            labResult = await _userManager.AddUserAndTimestamp(labResult, User, DbEnum.DbActionEnum.Create);
                            var _labResult = await _labResultRepository.AddAsync(labResult);
                            if (_labResult != null)
                            {
                                await _branchService.IncreaseVoucherNo(VoucherTypeEnum.LabResult);
                                if(labResult.LabOrderId !=0)
                                {
                                    foreach (var lab in labResult.LabResultDetails)
                                    {
                                        await _labOrderRepository.UpdateLabOrderTest(labResult.LabOrderId, _labResult.Id, lab.TestId);
                                    }
                                }
                                await transaction.CommitAsync();
                                TempData["notice"] = StatusEnum.NoticeStatus.Success;
                                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));
                                return RedirectToAction(nameof(Index));
                            }
                            throw new Exception();
                        }
                        else
                        {
                            labResult = await _userManager.AddUserAndTimestamp(labResult, User, DbEnum.DbActionEnum.Create);
                            var _labResult = await _labResultRepository.AddAsync(labResult);
                            if (_labResult != null)
                            {
                                await _branchService.IncreaseVoucherNo(VoucherTypeEnum.LabResult);
                                if (labResult.LabOrderId != 0)
                                {
                                    foreach(var lab in labResult.LabResultDetails)
                                    {
                                        await _labOrderRepository.UpdateLabOrderTest(labResult.LabOrderId, _labResult.Id, lab.TestId);
                                    }
                                }
                                await transaction.CommitAsync();
                                TempData["notice"] = StatusEnum.NoticeStatus.Success;
                                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));
                                return RedirectToAction(nameof(Index));
                            }
                            throw new Exception();
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    await transaction.RollbackAsync();
                }
            }
            Initialize(labResult);
            return View(labResult);
        }

        public IActionResult Edit(int id)
        {
            var labResult = _labResultRepository.Get(id);
            Initialize(labResult);
            return View(labResult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LabResult labResult)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    ModelState.Remove("TechnicianFeeType");
                    ModelState.Remove("ConsultantFeeType");
                    ModelState.Remove("LabOrderId");
                    if (ModelState.IsValid)
                    {
                        if (labResult.LabResultDetails != null && labResult.LabResultDetails.Count() > 0)
                        {
                            labResult = await _userManager.AddUserAndTimestamp(labResult, User, DbEnum.DbActionEnum.Update);
                            var _labResult = await _labResultRepository.UpdateAsync(labResult);
                            if (_labResult != null)
                            {
                                await transaction.CommitAsync();
                                TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {
                            labResult = await _userManager.AddUserAndTimestamp(labResult, User, DbEnum.DbActionEnum.Update);
                            var _labResult = await _labResultRepository.UpdateAsync(labResult);
                            if (_labResult != null)
                            {
                                await transaction.CommitAsync();
                                TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                                return RedirectToAction("Index");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.InnerException.Message);
                    await transaction.RollbackAsync();
                }
            }
            Initialize(labResult);
            return View(labResult);
        }

        public async Task<IActionResult> Delete(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var labResult = _labResultRepository.Get(id);
                    var isSucceed = await _labResultRepository.DeleteAsync(id);
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

        public async Task<IActionResult> Completed(int id)
        {
            var labResult = await _context.LabResults.FindAsync(id);
            if (labResult != null)
            {
                labResult.IsCompleted = true;
                labResult.CompletedDate = DateTime.Now;
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
                var labResult = _labResultRepository.Get(id);
                
                labResult.PatientGender = labResult.PatientSex == 1 ? "M": "F";
                labResult.DateString = labResult.Date.ToString("dd-MM-yyyy");
              
                List<LabResult> labResults = new List<LabResult>();
                labResults.Add(labResult);
                List<LabResultDetailDTO> labResultDetails = new List<LabResultDetailDTO>();
                var result = _context.LabResultDetails.Where(x => x.LabResultId == id).ToList();
               
                foreach(var detail in result)
                {
                    LabResultDetailDTO lRD = new LabResultDetailDTO();
                    var testName = _context.LabTests.Where(x => x.Id == detail.TestId).FirstOrDefault();
                    if (labResultDetails.Count>0)
                    {
                        if(labResultDetails[labResultDetails.Count-1].testId!=testName.Id)
                        {
                            lRD.Name = testName.Name;
                            lRD.testId = testName.Id;
                            labResultDetails.Add(lRD);
                        }
                    }
                    else
                    {
                        lRD.Name = testName.Name;
                        lRD.testId = testName.Id;
                        labResultDetails.Add(lRD);
                    }
                    LabResultDetailDTO labResultDetail = new LabResultDetailDTO();
                    
                    labResultDetail.Name = detail.Name;
                    if(detail.MinRange!=null || detail.MaxRange!=null)
                    {
                        labResultDetail.ReferenceRange = detail.MinRange.ToString() + "~" + detail.MaxRange.ToString();
                    }
                    // labResultDetail.MinRange = detail.MinRange;
                    labResultDetail.testId = detail.TestId;
                    labResultDetail.LabUnit = detail.LabUnit;
                    labResultDetail.Result = detail.Result;
                    labResultDetail.Remark = detail.Remark;
                    labResultDetails.Add(labResultDetail);
                }
                List<Branch> branches = new List<Branch>();
                var branch = _branchService.GetBranchById(labResult.BranchId);
                branches.Add(branch);

                report.DataSources.Add(new ReportDataSource("Branch",branches));
                report.DataSources.Add(new ReportDataSource("LabResultDataSet", labResults));
                report.DataSources.Add(new ReportDataSource("DataSet1", labResultDetails));
                report.ReportPath = $"{_webHostEnvironment.WebRootPath}\\ReportFiles\\LabResultReport.rdlc";

                var pdf = report.Render(renderFormat);
                return File(pdf, mimetype, labResult.PatientName+"_LabResult_"+DateTime.Now+"." + extension);
            }
        }

        public IActionResult DownloadSFReport()
        {
            var page = TempData["Page"];

            DateTime? fromDate = Convert.ToDateTime(TempData["FromDate"]);
            DateTime? toDate = Convert.ToDateTime(TempData["ToDate"]);
            string? ResultNo = null;
            int? TechnicianId = Convert.ToInt32(TempData["TechnicianId"]);
            int? ConsultantId = Convert.ToInt32(TempData["ConsultantId"]);
            int? PatientId = null;
            int? TestId = null;
            if (TempData["FromDate"] == null)
            {
                fromDate = null;
                toDate = null;

            }
            if (TempData["PatientId"] != null)
            {
                PatientId = (int)TempData["PatientId"];
            }
            if (TempData["ResultNo"] != null)
            {
                ResultNo = TempData["ResultNo"].ToString();
            }
            string renderFormat = "PDF";
            string extension = "pdf";
            string mimetype = "application/pdf";
            using (var report = new LocalReport())
            {
                List<LabResult> labResults = new List<LabResult>();
                List<Branch> branches = new List<Branch>();
                var user = _userService.Get(User);
                 labResults = _labResultRepository.GetAll(user.BranchId, null, PatientId, ResultNo, TechnicianId, ConsultantId, true, null, null, null, null, fromDate, toDate,null);
                foreach (var lab in labResults)
                {
                    if (lab.TechnicianFeeType == FeeTypeEnum.FixedAmount)
                    {
                        lab.TechanicianFeeTypeStr = "FixAmount";
                        lab.TechanicianAmount = lab.TechnicianFee;
                    }
                    else
                    {
                        lab.TechanicianFeeTypeStr =  "Percentage";
                        var perVal = lab.TechnicianFee / 100;
                        lab.TechanicianAmount = lab.UnitPrice * perVal;
                    }
                    if (lab.ConsultantFeeType == FeeTypeEnum.FixedAmount)
                    {
                        lab.ConsultantFeeTypeStr = "FixAmount";
                        lab.ConsultantAmount = lab.ConsultantFee;
                    }
                    else
                    {
                        lab.ConsultantFeeTypeStr = "Percentage";
                        var perVal = lab.ConsultantFee / 100;
                        lab.ConsultantAmount = lab.UnitPrice * perVal;
                    }

                }
                var branch = _branchService.GetBranchById((int)_userService.Get(User).BranchId);
                branches.Add(branch);
                ReportParameter[] parameters = new ReportParameter[2];
                parameters[0] = new ReportParameter("FromDate", fromDate.ToString());
                parameters[1] = new ReportParameter("ToDate", toDate.ToString());

                report.DataSources.Add(new ReportDataSource("Branch", branches));
                report.DataSources.Add(new ReportDataSource("ServiceFeesDataset", labResults));
                report.ReportPath = $"{_webHostEnvironment.WebRootPath}\\ReportFiles\\ServiceFeesReport.rdlc";
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                TempData["Page"] = page;
                TempData["FromDate"] = fromDate;
                TempData["ToDate"] = toDate;
                TempData["ResultNo"] = ResultNo;
                TempData["TechnicianId"] = TechnicianId;
                TempData["ConsultantId"] = ConsultantId;
                return File(pdf, mimetype, "_ServiceFeesReport_" + DateTime.Now + "." + extension);
            }
        }

        //public IActionResult PrintSlip(int id)
        //{
        //    string renderFormat = "PDF";
        //    string extension = "pdf";
        //    string mimetype = "application/pdf";
        //    using (var report = new LocalReport())
        //    {
        //        var labResult = _labResultRepository.Get(id);
        //        var labResultTests = labResult.LabResultDetails.Select(x => new
        //        {
        //            Item = x.LabResultNo,
        //            UnitPrice = x.UnitPrice.ToString("0.00"),
        //            Qty = x.Qty,
        //            Price = (x.UnitPrice * x.Qty).ToString("0.00"),
        //            ResultNo = labResult.ResultNo,
        //            Date = labResult.Date.ToString("dd-MM-yyyy"),
        //            IsCompleted = labResult.IsCompleted,
        //            Tax = labResult.Tax.ToString("0.00"),
        //            Discount = labResult.Discount.ToString("0.00"),
        //            PatientName = labResult.PatientName,
        //            BranchName = labResult.BranchName,
        //            Address = labResult.BranchAddress,
        //            Phone = labResult.BranchPhone,
        //            UnitShortForm = ""


        //        }).ToList();
        //        report.DataSources.Add(new ReportDataSource("dsPharmacyReceipt", labResultTests));
        //        report.ReportPath = $"{_webHostEnvironment.WebRootPath}\\ReportFiles\\PharmacyReceipt80mm.rdlc";

        //        var pdf = report.Render(renderFormat);
        //        return File(pdf, mimetype, "report." + extension);
        //    }
        //}

        public IActionResult GetAll(int? PatientId = null)
        {
            var labResults = _labResultRepository.GetAll(_userService.Get(User).BranchId, PatientId: PatientId);
            return Ok(labResults);
        }
    }
}