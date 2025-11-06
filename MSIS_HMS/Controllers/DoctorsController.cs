using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Enums;
using MSIS_HMS.Helpers;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Interfaces;
using MSIS_HMS.Models;
using X.PagedList;


namespace MSIS_HMS.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISpecialityRepository _specialityRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;
        private readonly Pagination _pagination;
        private readonly ILogger<DoctorsController> _logger;

        public DoctorsController(ISpecialityRepository specialityRepository, IDepartmentRepository departmentRepository, UserManager<ApplicationUser> userManager, IUserService userService, IDoctorRepository doctorRepository, ApplicationDbContext context, IOptions<Pagination> pagination, ILogger<DoctorsController> logger)
        {
            _userManager = userManager;
            _specialityRepository = specialityRepository;
            _doctorRepository = doctorRepository;
            _departmentRepository = departmentRepository;
            _userService = userService;
            _context = context;
            _pagination = pagination.Value;
            _logger = logger;
        }

        public void Initialize(Doctor doctor = null)
        {
            ViewData["Specialities"] = _specialityRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", doctor?.SpecialityId);
        }

        // GET
        public IActionResult Index(string DoctorName = null, string Code = null, string SamaNumber = null, int? DepartmentId = null, int? SpecialityId = null, int? page = 1)
        {
            Initialize();
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            var doctors = _doctorRepository.GetAll(_userService.Get(User).BranchId, null, DoctorName, Code, SamaNumber, DepartmentId, SpecialityId);

            foreach (var obj in doctors)
            {
                obj.Speciality = _context.Specialities.FirstOrDefault(u => u.Id == obj.SpecialityId);
            }
            ViewData["Departments"] = _departmentRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", DepartmentId);
            ViewData["Specialities"] = _specialityRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", SpecialityId);
            return View(doctors.OrderByDescending(doctor => doctor.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }

        public ActionResult Create()
        {
            Initialize();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Doctor doctor)
        {

            try
            {
                var filename = doctor.ImageFile != null ? FtpHelper.ftpDoctorImageFolderPath + doctor.ImageFile.GetUniqueName() : "";

                if (ModelState.IsValid)
                {
                    var didUploaded = true;
                    if (doctor.ImageFile != null)
                    {
                        didUploaded = false;
                        var uploadRes = FtpHelper.UploadFileToServer(doctor.ImageFile, filename);
                        if (uploadRes.IsSucceed())
                        {
                            didUploaded = true;
                            doctor.Image = uploadRes.ResponseUri.AbsolutePath;
                        }
                    }
                    if (didUploaded)
                    {
                        doctor = await _userManager.AddUserAndTimestamp(doctor, User, DbEnum.DbActionEnum.Create);
                        doctor = await _userManager.AddUserAndTimestamp(doctor, User, DbEnum.DbActionEnum.Update);
                        doctor.CFFeeForHospital = doctor.CFFeeForHospital / 100;
                        doctor.RoundFeeForHospital = doctor.RoundFeeForHospital / 100;
                        if (doctor.Schedules != null)
                        {
                            doctor.Schedules.ToList().ForEach(x => x.BranchId = doctor.BranchId);
                        }
                        var _doctor = await _doctorRepository.AddAsync(doctor);

                        if (_doctor != null)
                        {
                            TempData["notice"] = StatusEnum.NoticeStatus.Success;
                            _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));
                            return RedirectToAction(nameof(Index));
                        }
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
            var _doctors = _doctorRepository.Get(id);
            _doctors.CFFeeForHospital = _doctors.CFFeeForHospital * 100;
            _doctors.RoundFeeForHospital = _doctors.RoundFeeForHospital * 100;
            _doctors.ImageContent = _doctors.Image.GetBase64();
            return View(_doctors);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Doctor doctor)
        {
            try
            {
                var filename = doctor.ImageFile != null ? FtpHelper.ftpDoctorImageFolderPath + doctor.ImageFile.GetUniqueName() : "";
                var didUploaded = true;
                if (doctor.ImageFile != null)
                {
                    didUploaded = false;
                    var uploadRes = FtpHelper.UploadFileToServer(doctor.ImageFile, filename);
                    if (uploadRes.IsSucceed())
                    {
                        var didDeleted = true;
                        if (FtpHelper.CheckIfFileExistsOnServer(doctor.Image))
                        {
                            didDeleted = false;
                            var deleteRes = FtpHelper.DeleteFileOnServer(doctor.Image);
                            if (deleteRes.IsSucceed())
                            {
                                didDeleted = true;
                            }
                        }
                        if (didDeleted)
                        {
                            didUploaded = true;
                            doctor.Image = uploadRes.ResponseUri.AbsolutePath;
                        }
                    }
                }
                if (didUploaded)
                {
                    if (ModelState.IsValid)
                    {
                        doctor = await _userManager.AddUserAndTimestamp(doctor, User, DbEnum.DbActionEnum.Update);
                        doctor.CFFeeForHospital = doctor.CFFeeForHospital / 100;
                        doctor.RoundFeeForHospital = doctor.RoundFeeForHospital / 100;
                        var _doctor = await _doctorRepository.UpdateAsync(doctor);
                        TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                        _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }
            Initialize();
            return View(doctor);

        }

        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var _doctor = await _doctorRepository.DeleteAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Delete));
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult GetAll(int? DepartmentType = null, int? DepartmentId = null, int? SpecialityId = null)
        {
            var doctors = _doctorRepository.GetAll(_userService.Get(User).BranchId, DepartmentId: DepartmentId, DepartmentType: DepartmentType, SpecialityId: SpecialityId);
            return Ok(doctors);
        }
        public IActionResult GetDoctorTypeEnum()
        {
            List<OTDoctorTypeEnumDTO> oTDoctorTypeEnumDTOs = new List<OTDoctorTypeEnumDTO>();
            var doctorTypeEnum = Enum.GetValues(typeof(MSIS_HMS.Core.Enums.OTDoctorTypeEnum))
            .Cast<MSIS_HMS.Core.Enums.OTDoctorTypeEnum>().ToDictionary(k => k.ToString(), v => (int)v);
            //.Select(v => v.ToString()).ToList();
            foreach(var e in doctorTypeEnum)
            {
                OTDoctorTypeEnumDTO oTDoctorTypeEnumDTO = new OTDoctorTypeEnumDTO();
                oTDoctorTypeEnumDTO.Name = e.Key;
                oTDoctorTypeEnumDTO.value = e.Value;
                oTDoctorTypeEnumDTOs.Add(oTDoctorTypeEnumDTO);

            }
            return Ok(oTDoctorTypeEnumDTOs);
        }
        public IActionResult GetAvailableDoctors(int? DepartmentType = null, int? DepartmentId = null, int? SpecialityId = null)
        {
            var currentTime = DateTime.Now.TimeOfDay;
            var currentDay = DateTime.Now.DayOfWeek;
            //var docs = _context.Schedules.Include(x => x.Doctor).Include(x => x.Department).ToList();
            var available_doctors = _context.Schedules.Include(x => x.Doctor).Include(x => x.Department).Where(x =>
                    x.BranchId == _userService.Get(User).BranchId &&
                    (DepartmentType == null || (int)x.Department.Type == (int)DepartmentType) &&
                    (DepartmentId == null || x.DepartmentId == DepartmentId) &&
                    (SpecialityId == null || x.Doctor.SpecialityId == SpecialityId) &&
                    x.DayOfWeek == currentDay &&
                    currentTime >= x.FromTime && currentTime <= x.ToTime
                ).AsEnumerable().Select(x =>
                {
                    var patientInQueue = GetPatientInQueue(x.DoctorId);
                    return new AvailableDoctor()
                    {
                        Id = x.DoctorId,
                        Name = x.Doctor.Name,
                        PatientInQueue = patientInQueue,
                        EstWaitingTime = GetEstWaitingTime(patientInQueue,x.DoctorId),
                        FromTime = x.FromTime,
                        ToTime = x.ToTime
                    };
                }).ToList();

            return Ok(available_doctors);
        }

        public int GetPatientInQueue(int DoctorId)
        {
            // && DateTime.Equals(x.Date.Date, DateTime.Now.Date)
            var patientInQueue = _context.Visits.ToList().Where(x => !x.IsDelete && x.Date.Date == DateTime.Now.Date && x.DoctorId == DoctorId && x.Status == Core.Enums.VisitStatusEnum.Booked).ToList();
            return patientInQueue.Count();
        }

        public string GetEstWaitingTime(int PatientInQueue,int DoctorId)
        {
            var DoctorInfo = _context.Doctors.Where(x => x.Id == DoctorId).FirstOrDefault();
            var waitingTimePerPatient = DoctorInfo.EstimatewaitingTime; // 8 min per patient
            var totalMin = PatientInQueue * waitingTimePerPatient;
            if (totalMin < 60)
            {
                return string.Format("{0}min", totalMin);
            }
            TimeSpan estWaitingTime = TimeSpan.FromMinutes(totalMin);
            return estWaitingTime.Minutes > 0 ? string.Format("{0}hr {1}min", estWaitingTime.Hours, estWaitingTime.Minutes) : string.Format("{0}hr", estWaitingTime.Hours);
        }
    }
}