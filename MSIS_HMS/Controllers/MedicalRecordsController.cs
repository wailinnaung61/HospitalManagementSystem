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
using MSIS_HMS.Helpers;
using MSIS_HMS.Core.Enums;

namespace MSIS_HMS.Controllers
{
    public class MedicalRecordsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IVisitRepository _visitRepository;
        private readonly IUserService _userService;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IOutletRepository _outletRepository;
        private readonly IBranchService _branchService;
        private readonly ILogger<MedicalRecordsController> _logger;
        private readonly Pagination _pagination;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IIPDRecordRepository _iPDRecordRepository;

        public MedicalRecordsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IBranchService branchService, IMedicalRecordRepository medicalRecordRepository, IVisitRepository visitRepository, IUserService userService, ILogger<MedicalRecordsController> logger, IOptions<Pagination> pagination, IDoctorRepository doctorRepository, IPatientRepository patientRepository, IOutletRepository outletRepository, IWebHostEnvironment webHostEnvironment,IIPDRecordRepository iPDRecordRepository)
        {
            _userManager = userManager;
            _context = context;
            _medicalRecordRepository = medicalRecordRepository;
            _visitRepository = visitRepository;
            _userService = userService;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _outletRepository = outletRepository;
            _branchService = branchService;
            _logger = logger;
            _pagination = pagination.Value;
            _webHostEnvironment = webHostEnvironment;
            _iPDRecordRepository = iPDRecordRepository;
        }

        public void Initialize(MedicalRecord medicalRecord = null)
        {
            ViewData["Patients"] =new SelectList( _patientRepository.GetAll(_userService.Get(User).BranchId),"Id", "Name",medicalRecord?.PatientId);
            ViewData["Doctors"] = _doctorRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", medicalRecord?.DoctorId);
        }

        // GET
        public IActionResult Index(int PatientId, int? page = 1, int? MedicalRecordId = null, int? DoctorId = null, int? VisitId = null, DateTime? StartDate = null, DateTime? EndDate = null,int? IPDRecordId=null)
        {
            var user = _userService.Get(User);
            if(TempData["PatientId"]!=null && PatientId==0)
            {
                PatientId = Convert.ToInt32(TempData["PatientId"]);
            }

            var iPDRecords = _iPDRecordRepository.Get(Convert.ToInt32(IPDRecordId));
            if(IPDRecordId!=null)
            {
                PatientId = iPDRecords.PatientId;
                ViewData["IPDRecordId"] = IPDRecordId;
            }
            var medicalRecords = _medicalRecordRepository.GetAll(user.BranchId, null, null, null, StartDate, EndDate, PatientId, DoctorId);
            if(PatientId!=0)
            {
                ViewData["PatientId"] = PatientId;
                ViewData["PatientName"] = _patientRepository.Get(PatientId).Name;

            }
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["Doctors"] = _doctorRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", DoctorId);
            ViewData["DoctorId"] = _doctorRepository.GetAll(_userService.Get(User).BranchId).FirstOrDefault();
            TempData["PatientId"] = null;
            return View(medicalRecords.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }

        public IActionResult Visit(int VisitId)
        {
            var medicalRecords = _medicalRecordRepository.GetAll(_userService.Get(User).BranchId, VisitId: VisitId);
            if (medicalRecords != null && medicalRecords.Count() > 0) // Already Exist
            {
                return RedirectToAction("Edit", new { id = medicalRecords.First().Id });
            }
            else
            {
                var visit = _visitRepository.Get(VisitId);
                return RedirectToAction("Create", new { PatientId = visit.PatientId, DoctorId = visit.DoctorId, VisitId = visit.Id, DepartmentType = (int)DepartmentTypeEnum.OPD });
            }
        }
       
        public IActionResult Create(int? PatientId = null, int? DoctorId = null, int? DepartmentType = null,int? IPDRecordId=null)
        {
            
            MedicalRecord medicalRecord = new MedicalRecord();
           
            if (IPDRecordId!=null)
            {
                var ipdRecord = _iPDRecordRepository.Get(Convert.ToInt32(IPDRecordId));
               medicalRecord.PatientId = ipdRecord.PatientId;
                PatientId = ipdRecord.PatientId;
                DoctorId = (int)_userService.Get(User).DoctorId;
                DepartmentType = (int)DepartmentTypeEnum.IPD;
            }
            else
            {
                DepartmentType = (int)DepartmentTypeEnum.OPD;
            }
            TempData["PatientId"] = PatientId;
            Initialize(medicalRecord);
            return View(new MedicalRecord(PatientId, DoctorId, DepartmentType,IPDRecordId));
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicalRecord medicalRecord)
        {
            
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var filename = !string.IsNullOrEmpty(medicalRecord.DoctorNotesImgContent) ? FtpHelper.ftpDoctorNotesImageFolderPath + FtpHelper.GetUniqueName() : "";
                try
                {
                    ModelState.Remove("DoctorId");
                    if (ModelState.IsValid)
                    {
                        var didUploaded = true;
                        if (!string.IsNullOrEmpty(medicalRecord.DoctorNotesImgContent))
                        {
                            didUploaded = false;
                            var uploadRes = FtpHelper.UploadFileToServer(medicalRecord.DoctorNotesImgContent, filename);
                            if (uploadRes.IsSucceed())
                            {
                                didUploaded = true;
                                medicalRecord.DoctorNotesImg = uploadRes.ResponseUri.AbsolutePath;
                            }
                        }
                        if (didUploaded)
                        {
                            medicalRecord = await _userManager.AddUserAndTimestamp(medicalRecord, User, DbEnum.DbActionEnum.Create);
                            if (medicalRecord.DoctorId <= 0)
                            {
                                medicalRecord.DoctorId = (int)_userService.Get(User).DoctorId;
                            }
                            var _medicalRecord = await _medicalRecordRepository.AddAsync(medicalRecord);
                            if (_medicalRecord != null)
                            {
                                await transaction.CommitAsync();
                                TempData["notice"] = StatusEnum.NoticeStatus.Success;
                                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));
                                return RedirectToAction(nameof(Index), new { PatientId = _medicalRecord.PatientId });
                            }
                        }
                        throw new Exception();
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    await transaction.RollbackAsync();
                    if (FtpHelper.CheckIfFileExistsOnServer(filename))
                    {
                        var deleteRes = FtpHelper.DeleteFileOnServer(filename);
                        if (deleteRes.IsSucceed())
                        {
                            _logger.LogError("Not successful delete item thumbnail");
                            Console.WriteLine(e.Message);
                        }
                    }
                }
            }
            Initialize(medicalRecord);
            return View(medicalRecord);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Create(MedicalRecord medicalRecord)
        //{
        //    Initialize(medicalRecord);
        //    return View(medicalRecord);
        //}

        public IActionResult Edit(int id)
        {
            var medicalRecord = _medicalRecordRepository.Get(id);
            medicalRecord.DoctorNotesImgContent = medicalRecord.DoctorNotesImg.GetBase64();
            Initialize(medicalRecord);
            return View(medicalRecord);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MedicalRecord medicalRecord)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var filename = !string.IsNullOrEmpty(medicalRecord.DoctorNotesImgContent) ? FtpHelper.ftpDoctorNotesImageFolderPath + FtpHelper.GetUniqueName() : "";
                try
                {
                    if (ModelState.IsValid)
                    {
                        var didUploaded = true;
                        if (medicalRecord.DoctorNotesImgContent != null)
                        {
                            didUploaded = false;
                            var uploadRes = FtpHelper.UploadFileToServer(medicalRecord.DoctorNotesImgContent, filename);
                            if (uploadRes.IsSucceed())
                            {
                                var didDeleted = true;
                                if (FtpHelper.CheckIfFileExistsOnServer(medicalRecord.DoctorNotesImg))
                                {
                                    didDeleted = false;
                                    var deleteRes = FtpHelper.DeleteFileOnServer(medicalRecord.DoctorNotesImg);
                                    if (deleteRes.IsSucceed())
                                    {
                                        didDeleted = true;
                                    }
                                }
                                if (didDeleted)
                                {
                                    didUploaded = true;
                                    medicalRecord.DoctorNotesImg = uploadRes.ResponseUri.AbsolutePath;
                                }
                            }
                        }
                        if(didUploaded)
                        {
                            medicalRecord = await _userManager.AddUserAndTimestamp(medicalRecord, User, DbEnum.DbActionEnum.Update);
                            var _medicalRecord = await _medicalRecordRepository.UpdateAsync(medicalRecord);
                            if (_medicalRecord != null)
                            {
                                await transaction.CommitAsync();
                                TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                                return RedirectToAction("Index", new { PatientId = _medicalRecord.PatientId });
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("Not successful save Blog");
                    Console.WriteLine(e.Message);
                    await transaction.RollbackAsync();
                    if (FtpHelper.CheckIfFileExistsOnServer(filename))
                    {
                        var deleteRes = FtpHelper.DeleteFileOnServer(filename);
                        if (deleteRes.IsSucceed())
                        {
                            _logger.LogError("Not successful delete item thumbnail");
                            Console.WriteLine(e.Message);
                        }
                    }
                }
            }
            Initialize(medicalRecord);
            return View(medicalRecord);
        }

        public async Task<IActionResult> Delete(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var medicalRecord = await _context.MedicalRecords.FindAsync(id);
                    if (medicalRecord == null)
                    {
                        return BadRequest();
                    }
                    medicalRecord.IsDelete = true;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return RedirectToAction(nameof(Index), new { PatientId = medicalRecord.PatientId });
                }
                catch (Exception e)
                {
                    _logger.LogError(e.InnerException.Message);
                    await transaction.RollbackAsync();
                    TempData["notice"] = StatusEnum.NoticeStatus.Fail;
                    return BadRequest();
                }
            }
        }

        public IActionResult MedicalRecordDetail(int id)
        {
            var medicalRecord = _medicalRecordRepository.Get(id);
            medicalRecord.DoctorNotesImgContent = medicalRecord.DoctorNotesImg.GetBase64();
            Initialize(medicalRecord);
            return View(medicalRecord);
        }

        public IActionResult PrintReceipt(int id)
        {
         
            string renderFormat = "PDF";
            string extension = "pdf";
            string mimetype = "application/pdf";
            using (var report = new LocalReport())
            {
                List<MedicalRecord> medicalRecords = new List<MedicalRecord>();
              
                var medicalRecord = _medicalRecordRepository.Get(id);
                var patientInfo = _patientRepository.Get(medicalRecord.PatientId);
                if(patientInfo.DateOfBirth==null)
                {
                    medicalRecord.PatientAge =(int) patientInfo.AgeYear;
                }
                else
                {
                    medicalRecord.PatientAge = (int)(DateTime.Now.Year - Convert.ToDateTime(patientInfo.DateOfBirth).Year);
                }
                medicalRecord.DateString = medicalRecord.Date.ToString("dd-MM-yyyy");
                var symptom = medicalRecord.PatientSymptoms;
                var diagnostic = medicalRecord.PatientDiagnostics;
                var prescription = medicalRecord.Prescriptions;
                medicalRecords.Add(medicalRecord);
                List<Branch> branches = new List<Branch>();
                var branch = _branchService.GetBranchById(medicalRecord.BranchId);
                branches.Add(branch);
                report.DataSources.Add(new ReportDataSource("MedicalRecord", medicalRecords));
                report.DataSources.Add(new ReportDataSource("Symptom", symptom));
                report.DataSources.Add(new ReportDataSource("Diagnostic", diagnostic));
                report.DataSources.Add(new ReportDataSource("Prescription", prescription));
                report.DataSources.Add(new ReportDataSource("Branch", branches));
                report.ReportPath = $"{_webHostEnvironment.WebRootPath}\\ReportFiles\\MedicalRecord.rdlc";

                var pdf = report.Render(renderFormat);
                return File(pdf, mimetype, medicalRecord.PatientName+"_MedicalRecord_"+DateTime.Now+"." + extension);
            }
        }
    }
}