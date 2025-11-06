using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using ZXing;
using ZXing.QrCode;

namespace MSIS_HMS.Controllers
{
    public class PatientsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IPatientRepository _patientRepository;
        private readonly IPatientResultImageRepository _patientResultImageRepository;
        private readonly IReferrerRepository _referrerRepository;
        private readonly IBranchService _branchService;
        private readonly IUserService _userService;
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IVisitRepository _visitRepository;
        private readonly Pagination _pagination;
        private readonly ILogger<PatientsController> _logger;


        public PatientsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IPatientRepository patientRepository, IReferrerRepository referrerRepository, IBranchService branchService, IUserService userService, IOptions<Pagination> pagination, ILogger<PatientsController> logger,IMedicalRecordRepository medicalRecordRepository,IVisitRepository visitRepository,IPatientResultImageRepository patientResultImageRepository)
        {
            _userManager = userManager;
            _context = context;
            _patientRepository = patientRepository;
            _referrerRepository = referrerRepository;
            _branchService = branchService;
            _userService = userService;
            _pagination = pagination.Value;
            _logger = logger;
            _medicalRecordRepository = medicalRecordRepository;
            _visitRepository = visitRepository;
            _patientResultImageRepository = patientResultImageRepository;
        }

        public void Initialize(Patient patient = null)
        {
            var countries = _context.Countries.Where(x => x.IsDelete == false).ToList();
            ViewData["Countries"] = new SelectList(countries, "Id", "Name", patient?.CountryId);
            var states = _context.States.Where(x=> x.IsDelete == false && (patient == null || x.CountryId==patient.CountryId)).ToList();
            ViewData["States"] = new SelectList(states, "Id", "Name", patient?.StateId);
            var cities = _context.Cities.Where(x => x.IsDelete == false && (patient == null || x.StateId == patient.StateId)).ToList();
            ViewData["Cities"] = new SelectList(cities, "Id", "Name", patient?.CityId);
            var townships = _context.Townships.Where(x => x.IsDelete == false && (patient==null || x.CityId==patient.CityId)).ToList();
            ViewData["Townships"] = new SelectList(townships, "Id", "Name", patient?.TownshipId);
            ViewData["Referrers"] = _referrerRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", patient?.ReferrerId);
        }

        // GET
        public IActionResult Index(DateTime? StartRegDate = null, DateTime? EndRegDate = null, string RegNo = null, string Name = null, string NRC = null, string Guardian = null, DateTime? DateOfBirth = null, string Phone = null, string BloodType = null,string Code= null, int? page = 1)
        {

            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            var patients = _patientRepository.GetAll(_userService.Get(User).BranchId, null, StartRegDate, EndRegDate, RegNo, Name, NRC, Guardian, DateOfBirth, Phone, BloodType, Code,null).ToList();
            var branches = _branchService.GetAll();
            patients.ForEach(x => x.Branch = branches.SingleOrDefault(b => b.Id == x.BranchId));
            return View(patients.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }

        public IActionResult Create()
        {
            Initialize();
            var patient = new Patient
            {
                RegNo = _branchService.GetVoucherNo(VoucherTypeEnum.Patient)
            };
            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient, bool? RedirectToVisit = null)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var filename = patient.ImageFile != null ? FtpHelper.ftpPatientImageFolderPath + patient.ImageFile.GetUniqueName() : "";
                    if (ModelState.IsValid)
                    {
                        var didUploaded = true;
                        if (patient.ImageFile != null)
                        {
                            didUploaded = false;
                            var uploadRes = FtpHelper.UploadFileToServer(patient.ImageFile, filename);
                            if (uploadRes.IsSucceed())
                            {
                                didUploaded = true;
                                patient.Image = uploadRes.ResponseUri.AbsolutePath;
                            }
                        }
                        if (didUploaded)
                        {
                            var branch = _branchService.GetBranchByUser();
                            patient.RegNo = _branchService.GetVoucherNo(VoucherTypeEnum.Patient, patient.RegNo);
                            if (string.IsNullOrEmpty(patient.RegNo))
                            {
                                ModelState.AddModelError("VoucherNo", "This field is required.");
                                Initialize(patient);
                                return View(patient);
                            }
                            patient = await _userManager.AddUserAndTimestamp(patient, User, DbEnum.DbActionEnum.Create);
                            patient.BarCode = patient.RegNo;
                            
                            string formatted = patient.RegDate.ToString("yyyy-MM-dd");

                            patient.QRCode = formatted + " " + patient.RegNo;
                            var _patient = await _patientRepository.AddAsync(patient);
                            if (_patient != null)
                            {
                                await _branchService.IncreaseVoucherNo(VoucherTypeEnum.Patient);
                                await transaction.CommitAsync();
                                if (RedirectToVisit == true)
                                {
                                    return RedirectToAction("Create", "Visits", new { PatientId = _patient.Id });
                                }
                                TempData["notice"] = StatusEnum.NoticeStatus.Success;
                                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        throw new Exception();
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.InnerException.Message);
                    await transaction.RollbackAsync();
                }
            }
            Initialize(patient);
            return View();
        }

        public IActionResult Edit(int id)
        {
            var patient = _patientRepository.Get(id);
            //var countries = _context.Countries.Where(x => x.IsDelete == false).ToList();
            //var states = _context.States.Where(x =>x.CountryId==patient.CountryId && x.IsDelete == false).ToList();
            //var cities = _context.Cities.Where(x => x.StateId == patient.StateId && x.IsDelete == false).ToList();
            patient.ImageContent = patient.Image.GetBase64();
            Initialize(patient);
            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Patient patient, bool? RedirectToVisit = null)
        {
            try
            {
                var filename = patient.ImageFile != null ? FtpHelper.ftpItemImageFolderPath + patient.ImageFile.GetUniqueName() : "";

                if (ModelState.IsValid)
                {
                    var didUploaded = true;
                    if (patient.ImageFile != null)
                    {
                        didUploaded = false;
                        var uploadRes = FtpHelper.UploadFileToServer(patient.ImageFile, filename);
                        if (uploadRes.IsSucceed())
                        {
                            var didDeleted = true;
                            if (FtpHelper.CheckIfFileExistsOnServer(patient.Image))
                            {
                                didDeleted = false;
                                var deleteRes = FtpHelper.DeleteFileOnServer(patient.Image);
                                if (deleteRes.IsSucceed())
                                {
                                    didDeleted = true;
                                }
                            }
                            if (didDeleted)
                            {
                                didUploaded = true;
                                patient.Image = uploadRes.ResponseUri.AbsolutePath;
                            }
                        }
                    }
                    if (didUploaded)
                    {
                        patient = await _userManager.AddUserAndTimestamp(patient, User, DbEnum.DbActionEnum.Update);
                        var _patient = await _patientRepository.UpdateAsync(patient);
                        if (RedirectToVisit == true)
                        {
                            return RedirectToAction("Create", "Visits", new { PatientId = _patient.Id });
                        }
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
            Initialize(patient);
            return View(patient);
        }

       
        public IActionResult PatientDetail(int patientId)
        {
            var user = _userService.Get(User);
            //var patient = _context.Patients.Where(x => x.Id == patientId).FirstOrDefault();
            PatientDetailDTO patientDetailDTO = new PatientDetailDTO();
            patientDetailDTO.Id = patientId;
            var visits = _visitRepository.GetAll(_userService.Get(User).BranchId, null, null, null, null, null, patientId, null, null, null);//_context.Visits.Where(x => x.PatientId == patientId).ToList();
            patientDetailDTO.visits = visits;
            var medicalRecords = _medicalRecordRepository.GetAll(user.BranchId, null, null, null, null, null, patientId, null); //_context.MedicalRecords.Where(x => x.PatientId == patientId).ToList();
            patientDetailDTO.medicalRecords = medicalRecords;
            var ipdRecords = _context.IPDRecords.Where(x => x.PatientId == patientId).ToList();
            patientDetailDTO.iPDRecords = ipdRecords;
            return View(patientDetailDTO);
        }
        [HttpGet]
        public IActionResult GetPatientDetail(int patientId)
        {
            var patient = _context.Patients.Where(x => x.Id == patientId).FirstOrDefault();
            PatientDetailDTO patientDetailDTO = new PatientDetailDTO();
            patientDetailDTO.Id = patient.Id;
            patientDetailDTO.Name = patient.Name;
            patientDetailDTO.Nrc = patient.NRC;
            patientDetailDTO.Address = patient.Address;
            patientDetailDTO.DateOfBirth = patient.DateOfBirth;
            patientDetailDTO.Gender =  patient.Gender.ToString();
            patientDetailDTO.Religion = patient.Religion;
            patientDetailDTO.Phone = patient.Phone;
            patientDetailDTO.Image = patient.Image.GetBase64();

            return Ok(patientDetailDTO);
        }
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var _patient = await _patientRepository.DeleteAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Delete));
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult GetState(int? countryId)
        {
            var states = _context.States.Where(x => x.CountryId == countryId && x.IsDelete == false).ToList();
            return Ok(states);
        }

        public IActionResult GetCity(int? stateId)
        {
            var cities = _context.Cities.Where(x => x.StateId == stateId && x.IsDelete == false).ToList();
            return Ok(cities);
        }
        public IActionResult GetTownship(int? cityId)
        {
            var townships = _context.Townships.Where(x => x.CityId == cityId && x.IsDelete == false).ToList();
            return Ok(townships);
        }
        public ActionResult Image(string path)
        {
            return File(FtpHelper.DownloadFileFromServer(path), "image/png");
        }

        public IActionResult ResultImage()
        {
            ViewData["Patients"] = _patientRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name");

            return View();
        }

        [HttpGet]
        public IActionResult GenerateQRCode(string regno,string date)
        {
            Byte[] byteArray;
            var width = 250; // width of the Qr Code   
            var height = 250; // height of the Qr Code   
            var margin = 0;
            var qrCodeWriter = new ZXing.BarcodeWriterPixelData
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = margin
                }
            };

            string qrtext = date + " " + regno;
            var pixelData = qrCodeWriter.Write(qrtext);

            // creating a bitmap from the raw pixel data; if only black and white colors are used it makes no difference   
            // that the pixel data ist BGRA oriented and the bitmap is initialized with RGB   
            using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            {
                using (var ms = new MemoryStream())
                {
                    var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    try
                    {
                        // we assume that the row stride of the bitmap is aligned to 4 byte multiplied by the width of the image   
                        System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }
                    // save to stream as PNG   
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byteArray = ms.ToArray();
                    string base64String = "data:image/png;base64," + Convert.ToBase64String(byteArray, 0, byteArray.Length);
                    return Ok(base64String);

                }
            }           
        }
        public IActionResult GenerateBarcode(string regno)
        {
            Byte[] byteArray;
            var width = 250; // width of the Qr Code   
            var height = 250; // height of the Qr Code   
            var margin = 0;
            var qrCodeWriter = new ZXing.BarcodeWriterPixelData
            {
                Format = ZXing.BarcodeFormat.CODE_93,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = margin
                }
            };
            var pixelData = qrCodeWriter.Write(regno);

            // creating a bitmap from the raw pixel data; if only black and white colors are used it makes no difference   
            // that the pixel data ist BGRA oriented and the bitmap is initialized with RGB   
            using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            {
                using (var ms = new MemoryStream())
                {
                    var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    try
                    {
                        // we assume that the row stride of the bitmap is aligned to 4 byte multiplied by the width of the image   
                        System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }
                    // save to stream as PNG   
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byteArray = ms.ToArray();
                    string base64String = "data:image/png;base64," + Convert.ToBase64String(byteArray, 0, byteArray.Length);
                    return Ok(base64String);

                }
            }
        }

    }
}   