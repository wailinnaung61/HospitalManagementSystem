using DevExpress.Utils.OAuth.Provider;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using X.PagedList;

namespace MSIS_HMS.Controllers
{
    public static class SessionExtensions
    {
        public static T GetComplexData<T>(this ISession session, string key)
        {
            var data = session.GetString(key);
            if (data == null)
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(data);
        }

        public static void SetComplexData(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
    }
    public class PatientResultImageController : Controller
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
        private readonly ILogger<PatientResultImageController> _logger;


        public PatientResultImageController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IPatientRepository patientRepository, IReferrerRepository referrerRepository, IBranchService branchService, IUserService userService, IOptions<Pagination> pagination, ILogger<PatientResultImageController> logger, IMedicalRecordRepository medicalRecordRepository, IVisitRepository visitRepository, IPatientResultImageRepository patientResultImageRepository)
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
        public IActionResult Create()
        {
            ViewData["Patients"] = _patientRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name");

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(PatientImageDTO patientResultImage)
        {
            try
            {
                for (int i = 0; i < patientResultImage.AttachmentPath.Count; i++)
                {
                    PatientResultImage patientResult = new PatientResultImage();
                    patientResult.PatientId = patientResultImage.PatientId;
                    patientResult.Name = patientResultImage.Name[i].ToString();
                    patientResult.AttachmentPath = patientResultImage.AttachmentPath[i].ToString();
                    patientResult.Remark = patientResult.Remark;
                    patientResult = await _userManager.AddUserAndTimestamp(patientResult, User, DbEnum.DbActionEnum.Create);
                    var result = await _patientResultImageRepository.AddAsync(patientResult);

                }
                ViewData["Patients"] = _patientRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

            }

            ViewData["Patients"] = _patientRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name");
            return View();
        }

        public IActionResult Edit(int id)
        {
            var order = _patientResultImageRepository.Get(id);
            ViewData["Patients"] = _patientRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name");
            return View(order);
        }

        public IActionResult Index(DateTime? StartDate = null, DateTime? EndDate = null, int? PatientId = null, int? page = 1)
        {
            ViewData["Patients"] = _patientRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name");
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            var patients = _patientResultImageRepository.GetAll(_userService.Get(User).BranchId, PatientId, StartDate, EndDate).ToList();
            var branches = _branchService.GetAll();
            patients.ForEach(x => x.Branch = branches.SingleOrDefault(b => b.Id == x.BranchId));
            return View(patients.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));

        }
    
        public async Task<IActionResult> DownloadFile(string path)
        {
            try
            {
                byte[] fileData;


                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FtpHelper.ftpurl + path);
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                //Enter FTP Server credentials.
                request.Credentials = new NetworkCredential(FtpHelper.ftpusername, FtpHelper.ftppassword);
                request.UsePassive = true;
                request.UseBinary = true;
                request.EnableSsl = false;

                //Fetch the Response and read it into a MemoryStream object.
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                using (MemoryStream stream = new MemoryStream())
                {
                    //Download the File.
                    response.GetResponseStream().CopyTo(stream);
                    fileData = stream.ToArray();
                }


                if (fileData != null)
                {

                    HttpContext.Session.SetComplexData("DownloadRecordData", null);

                    HttpContext.Session.SetComplexData("DownloadRecordData", fileData);
                    return Json("000");
                }
                else
                {
                    return Json("Please try again to download!");

                }
                //return File(FtpHelper.DownloadFileFromServer(path), "application/octet-stream");
            }
            catch(Exception ex)
            {
                return Json("");
            }
           
        }
        public ActionResult DownloadRecordData(string OriginalName)
        {
            if ((HttpContext.Session.GetComplexData<byte[]>("DownloadRecordData")) != null)
            {
                byte[] data = (HttpContext.Session.GetComplexData<byte[]>("DownloadRecordData")) as byte[];
                return File(data, "application/octet-stream", OriginalName);
            }
            else
            {
                return new EmptyResult();
            }

        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var _patient = await _patientResultImageRepository.DeleteAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Delete));
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }

            return RedirectToAction(nameof(Index));
        }




    }
}
