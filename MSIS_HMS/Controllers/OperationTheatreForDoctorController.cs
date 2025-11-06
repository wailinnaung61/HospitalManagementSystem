using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Reporting.NETCore;
using MSIS_HMS.Core.Entities;
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
using static MSIS_HMS.Infrastructure.Enums.DbEnum;

namespace MSIS_HMS.Controllers
{
    public class OperationTheatreForDoctorController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserService _userService;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IOutletRepository _outletRepository;
        private readonly IOperationRoomRepository _operationRoomRepository;
        private readonly IOperationTypeRepository _operationTypeRepository;
        private readonly IOperationTreaterRepository _operationTreaterRepository;
        private readonly IReferrerRepository _referrerRepository;
        private readonly IBranchService _branchService;
        private readonly ILogger<OperationTreater> _logger;
        private readonly Pagination _pagination;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public OperationTheatreForDoctorController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IOrderRepository orderRepository, IUserService userService, IPatientRepository patientRepository, IDoctorRepository doctorRepository, IOutletRepository outletRepository, IBranchService branchService, IWebHostEnvironment webHostEnvironment, ILogger<OperationTreater> logger, IOptions<Pagination> pagination, IOperationRoomRepository operationRoomRepository, IOperationTypeRepository operationTypeRepository, IOperationTreaterRepository operationTreaterRepository, IReferrerRepository referrerRepository)
        {
            _userManager = userManager;
            _context = context;
            _orderRepository = orderRepository;
            _userService = userService;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _outletRepository = outletRepository;
            _branchService = branchService;
            _logger = logger;
            _pagination = pagination.Value;
            _webHostEnvironment = webHostEnvironment;
            _operationRoomRepository = operationRoomRepository;
            _operationTypeRepository = operationTypeRepository;
            _operationTreaterRepository = operationTreaterRepository;
            _referrerRepository = referrerRepository;
        }

        public void Initialize(OperationTreater operationTreater = null)
        {
            ViewData["Patients"] = _patientRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", operationTreater?.PatientId);
            ViewData["Doctors"] = _doctorRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", operationTreater?.ChiefSurgeonDoctorId);
            ViewData["Aneasthetists"] = _doctorRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", operationTreater?.ChiefSurgeonDoctorId);
            ViewData["OperationRooms"] = _operationRoomRepository.GetAll(_userService.Get(User).BranchId, "Available").GetSelectListItems("Id", "RoomNo", operationTreater?.OperationRoomId);
            ViewData["OperationTypes"] = _operationTypeRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", operationTreater?.OpeartionTypeId);
            ViewData["Referrers"] = _referrerRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", operationTreater?.ReferrerId);
            var branch = _branchService.GetBranchByUser();
            ViewData["UseVoucherFormat"] = branch.UseVoucherFormatForOrder;
        }

        public IActionResult Index(int? page = 1, int? OrderId = null, int? PatientId = null, int? DoctorId = null, string VoucherNo = null, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            var user = _userService.Get(User);

            var orders = _operationTreaterRepository.GetAll(user.BranchId, user.OutletId, null, PatientId, DoctorId, VoucherNo, null, null, FromDate, ToDate, null, null, null);

            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            Initialize();
            return View(orders.OrderBy(x => x.IsPaid).ToList().ToPagedList((int)page, pageSize));
        }
        public IActionResult Create()
        {
            Initialize();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OperationTreater operationTreater)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    ModelState.Remove("OperationItems");
                    ModelState.Remove("OperationServices");
                    if (ModelState.IsValid)
                    {
                        //if (operationTreater.OperationItems != null && operationTreater.OperationItems.Count() > 0)
                        //{
                            var branch = _branchService.GetBranchByUser();

                            //if (branch.AutoPaidForOrder)
                            //{
                            //    order.IsPaid = true;
                            //    order.PaidDate = DateTime.Now;
                            //}
                            operationTreater.Total = operationTreater.OperationItems.CalculateTotal() + operationTreater.OperationServices.CalculateTotal() + operationTreater.OT_Doctors.CalculateFeeTotal() + operationTreater.OT_Staffs.CalculateFeeTotal() + operationTreater.ChiefSurgeonFee + (operationTreater.ReferrerFee != null ? (int)operationTreater.ReferrerFee : 0) + operationTreater.Tax - operationTreater.Discount; //_orderRepository.CalculateTotal(order.OrderItems.ToList());
                            operationTreater.ByDoctor = true;
                            operationTreater = await _userManager.AddUserAndTimestamp(operationTreater, User, DbEnum.DbActionEnum.Create);
                            operationTreater.OutletId = (int)_userService.Get(User).OutletId;
                            var _order = await _operationTreaterRepository.AddAsync(operationTreater);
                            if (_order != null)
                            {

                                var otRoom = await _context.OperationRooms.FindAsync(operationTreater.OperationRoomId);

                                otRoom.Status = "Not Available";
                                //await _context.SaveChangesAsync();
                                //await _outletRepository.UpdateStockAsync(operationTreater.OperationItems.GetOutletItemForUpdate(_operationTreaterRepository.GetOrderItemsForUpdate(_order.Id, _order.OutletId), _order.OutletId), (int)DbActionEnum.Delete);
                                // await _branchService.IncreaseVoucherNo(VoucherTypeEnum.Pharmacy);
                                await transaction.CommitAsync();
                                TempData["notice"] = StatusEnum.NoticeStatus.Success;
                                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));
                                return RedirectToAction(nameof(Index));
                            }
                            throw new Exception();
                        //}
                        //else
                        //{
                        //    ViewData["Error"] = "OperationTreater Items are required";
                        //}
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    await transaction.RollbackAsync();
                }
            }
            Initialize(operationTreater);
            return View(operationTreater);
        }
        public IActionResult Edit(int id)
        {
            var operationTreater = _operationTreaterRepository.Get(id);
            Initialize(operationTreater);
            return View(operationTreater);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OperationTreater operationTreater)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    ModelState.Remove("OperationItems");
                    ModelState.Remove("OperationServices");
                    if (ModelState.IsValid)
                    {
                        //if (operationTreater.OperationItems != null && operationTreater.OperationItems.Count() > 0)
                        //{
                            operationTreater.Total = operationTreater.OperationItems.CalculateTotal() + operationTreater.OperationServices.CalculateTotal() + operationTreater.OT_Doctors.CalculateFeeTotal() + operationTreater.OT_Staffs.CalculateFeeTotal() + operationTreater.ChiefSurgeonFee + (operationTreater.ReferrerFee != null ? (int)operationTreater.ReferrerFee : 0) + operationTreater.Tax - operationTreater.Discount; //_orderRepository.CalculateTotal(order.OrderItems.ToList());
                            operationTreater.ByDoctor = true;
                            operationTreater = await _userManager.AddUserAndTimestamp(operationTreater, User, DbEnum.DbActionEnum.Update);
                            operationTreater.OutletId = (int)_userService.Get(User).OutletId;
                            var _order = await _operationTreaterRepository.UpdateAsync(operationTreater);
                            if (_order != null)
                            {

                                var otRoom = await _context.OperationRooms.FindAsync(operationTreater.OperationRoomId);

                                otRoom.Status = "Not Available";
                                // await _outletRepository.UpdateStockAsync(operationTreater.OperationItems.GetOutletItemForUpdate(_operationTreaterRepository.GetOrderItemsForUpdate(_order.Id, _order.OutletId), _order.OutletId), (int)DbActionEnum.Delete);
                                await transaction.CommitAsync();
                                TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                                return RedirectToAction("Index");
                            }
                        //}
                        //else
                        //{
                        //    ViewData["Error"] = "Operation Items are required";
                        //}
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.InnerException.Message);
                    await transaction.RollbackAsync();
                }
            }
            Initialize(operationTreater);
            return View(operationTreater);
        }

        [HttpGet]
        public IActionResult PrintReceipt(int id)
        {
            string renderFormat = "PDF";
            string extension = "pdf";
            string mimetype = "application/pdf";
            using (var report = new LocalReport())
            {
                List<OperationTreater> operationTreaters = new List<OperationTreater>();
                var operationTreater = _operationTreaterRepository.Get(id);
                operationTreater.OperationType = operationTreater.Type.ToString();           
                operationTreaters.Add(operationTreater);
                var oT_Doctors = operationTreater.OT_Doctors.Select(x => new
                {

                    DoctorName = x.DoctorName,
                    OTDoctorTypeEnum = x.OTDoctorTypeEnum.ToString()

                }).ToList();
              
                var oT_Staffs = operationTreater.OT_Staffs.Select(x => new
                {

                    StaffName = x.StaffName,
                    OTStaffTypeEnum = x.OTStaffTypeEnum.ToString()

                }).ToList();
                var oT_Anaesthetists = operationTreater.OT_Anaesthetists.Select(x => new
                {

                    DoctorName = x.DoctorName,
                    OTDoctorTypeEnum = x.OTDoctorTypeEnum.ToString()

                }).ToList();

                List<Branch> branches = new List<Branch>();                 
                var branch = _branchService.GetBranchById(operationTreater.BranchId);
                branches.Add(branch);

                //report.DataSources.Add(new ReportDataSource("Branch", branches));
                report.DataSources.Add(new ReportDataSource("OTDoctor", oT_Doctors));
                report.DataSources.Add(new ReportDataSource("OTAneasthetist", oT_Anaesthetists));
                report.DataSources.Add(new ReportDataSource("OTNurse", oT_Staffs));
                report.DataSources.Add(new ReportDataSource("OTReportForDoctor", operationTreaters));
                //report.DataSources.Add(new ReportDataSource("dsOperationDoctorandStaff", OTDoctors.Union(OTStaffs)));
                report.ReportPath = $"{_webHostEnvironment.WebRootPath}\\ReportFiles\\OTReportForDoctor.rdlc";

                var pdf = report.Render(renderFormat);
                return File(pdf, mimetype, operationTreater.PatientName + "_OT_" + DateTime.Now + "." + extension);
            }

        }
    }
}
