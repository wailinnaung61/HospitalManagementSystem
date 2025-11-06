using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Enums;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Enums;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Interfaces;
using MSIS_HMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace MSIS_HMS.Controllers
{
    public class ImagingController : Controller
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

        public ImagingController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IBranchService branchService, ILabResultRepository labResultRepository, ILabPersonRepository labPersonRepository, ILabTestRepository labTestRepository, IUserService userService, ILogger<LabResultsController> logger, IOptions<Pagination> pagination, IDoctorRepository doctorRepository, IPatientRepository patientRepository, IOutletRepository outletRepository, IWebHostEnvironment webHostEnvironment, ILabOrderRepository labOrderRepository)
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
            ViewData["UseVoucherFormat"] = branch.UseVoucherFormatForImaging;
            ViewData["LabOrderNo"] = _labOrderRepository.GetLabOrderFromLabOrderTest(_userService.Get(User).BranchId).GetSelectListItems("Id", "VoucherNo");
        }
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
        public IActionResult Create()
        {
            Initialize();
            var labResult = new LabResult
            {
                ResultNo = _branchService.GetVoucherNo(VoucherTypeEnum.Imaging)
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
                            if (labResult.TechnicianId != null)
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
                            foreach (var labResultDetail in labResult.LabResultDetails)
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
                                if (labResult.LabOrderId != 0)
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

    }
}
