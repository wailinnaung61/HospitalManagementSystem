using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Enums;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSIS_HMS.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IAppointmentTypeRepository _appointmentTypeRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ApplicationDbContext _context;


        public AppointmentsController(UserManager<ApplicationUser> userManager, IDoctorRepository doctorRepository,IPatientRepository patientRepository,ApplicationDbContext context,IAppointmentTypeRepository appointmentTypeRepository,IAppointmentRepository appointmentRepository)
        {
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _appointmentTypeRepository = appointmentTypeRepository;
            _userManager = userManager;
            _appointmentRepository = appointmentRepository;
            _context = context;
        }
        public void Initialize(int? doctorId=null)
        {
            var doctors = _doctorRepository.GetAll(null,doctorId,null,null,null,null,null,null);
            ViewData["Doctors"] = new SelectList(doctors, "Id", "Name");
            var patients = _patientRepository.GetAll();
            ViewData["Patients"] = new SelectList(patients, "Id", "Name");
            var appointmentTypes = _appointmentTypeRepository.GetAll();
            ViewData["AppointmentTypes"] = new SelectList(appointmentTypes, "Id", "Type");

        }
        public IActionResult Index()
        {
            Initialize();

            return View();
        }
        public async Task<IActionResult> Create()
        {
            
            var user = await _userManager.GetUserAsync(User);
            if (user.DoctorId!=0)
            {
                Initialize(user.DoctorId);
            }
            else
            {
                Initialize();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var appointments = _appointmentRepository.GetAppointmentByDateTime(appointment.StartDate, appointment.EndDate);
                    //if(appointments.Count==0)
                    //{
                        if (appointment.Id == 0)
                        {
                            appointment = await _userManager.AddUserAndTimestamp(appointment, User, DbEnum.DbActionEnum.Create);
                            appointment.FromTime = appointment.StartDate.ToLongTimeString();
                            appointment.ToTime = appointment.EndDate.ToLongTimeString();
                            var _appointmentType = await _appointmentRepository.AddAsync(appointment);
                            if (_appointmentType != null)
                            {
                                TempData["notice"] = StatusEnum.NoticeStatus.Success;
                                //_logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));
                                return RedirectToAction(nameof(Create));
                            }
                        }
                        else
                        {
                            appointment = await _userManager.AddUserAndTimestamp(appointment, User, DbEnum.DbActionEnum.Update);
                            var _appointmentType = await _appointmentRepository.UpdateAsync(appointment);
                            if (_appointmentType != null)
                            {
                                TempData["notice"] = StatusEnum.NoticeStatus.Success;
                                //_logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));
                                return RedirectToAction(nameof(Create));
                            }
                        }
                    //}
                    //else
                    //{
                    //    return RedirectToAction(nameof(Create));
                    //}
                    
                }
            }
            catch (Exception ex)
            {

            }
            return View();
        }
        [HttpGet]
        public JsonResult GetCalendar(int doctorId)
        {
            var appointments = _appointmentRepository.GetAppointmentByDoctorId(doctorId);
            return Json(appointments);
        }
        [HttpGet]
        public IActionResult GetEventById(int id)
        {
            var appointment = _appointmentRepository.Get(id);           
            return Ok(appointment);
        }
        [HttpGet]
        public IActionResult GetAppointmentByDateTime(DateTime startDate,DateTime endDate)
        {
            var appointment = _appointmentRepository.GetAppointmentByDateTime(startDate, endDate);
            bool status = true;
            if (appointment.Count > 0)
            {
               
                foreach (var appoint in appointment)
                {
                    var sd = startDate.ToLongTimeString();
                    string[] sdate = sd.Split(':') ;
                    var shr = Convert.ToInt32(sdate[0]);
                    var smin = Convert.ToInt32(sdate[1]);
                   
                    var ed = endDate.ToLongTimeString();
                    string[] edate = ed.Split(':');
                    var ehr = Convert.ToInt32(edate[0]);
                    var emin = Convert.ToInt32(edate[1]);

                    string[] app = appoint.FromTime.Split(':');
                    var apphr = Convert.ToInt32(app[0]);
                    var appmin = Convert.ToInt32(app[1]);
                    //var appsec = Convert.ToInt32(app[2]);
                    string[] eapp = appoint.ToTime.Split(':');
                    var eapphr = Convert.ToInt32(eapp[0]);
                    var eappmin = Convert.ToInt32(eapp[1]);
                    //var eappsec = Convert.ToInt32(eapp[2]);

                    if (shr == apphr)
                    {
                        if(smin>=appmin)
                        {
                            status = false;
                        }
                        if(smin<appmin)
                        {
                            if(ehr>=eapphr)
                            {
                                status = false;
                            }
                        }
                        
                    }
                    if (shr > apphr && shr < eapphr)
                    {
                        status = false;
                    }
                    if(shr>apphr && shr==eapphr)
                    {
                        if(smin<eappmin)
                        {
                            status = false;
                        }
                    }
                    

                }
               
            }
            else
            {
                status = true;
            }
            if(status==true)
            {
                appointment = new List<Appointment>();
                return Ok(appointment);
            }
            else
            {
                return Ok(appointment);
            }
            //return Ok(appointment);
        }


    }
}
