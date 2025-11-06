using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Enums;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Interfaces;
using Microsoft.Extensions.Logging;
using MSIS_HMS.Models;
using Microsoft.Extensions.Options;
using X.PagedList;
using static MSIS_HMS.Infrastructure.Enums.DbEnum;
using Microsoft.Reporting.NETCore;
using System.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;

namespace MSIS_HMS.Controllers
{
    public class OrdersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserService _userService;
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IOutletRepository _outletRepository;
        private readonly IBranchService _branchService;
        private readonly ILogger<OrdersController> _logger;
        private readonly Pagination _pagination;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public OrdersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IBranchService branchService, IOrderRepository orderRepository, IUserService userService, ILogger<OrdersController> logger, IOptions<Pagination> pagination, IDoctorRepository doctorRepository, IPatientRepository patientRepository, IOutletRepository outletRepository, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _context = context;
            _orderRepository = orderRepository;
            _userService = userService;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _outletRepository = outletRepository;
            _branchService = branchService;
            _logger = logger;
            _pagination = pagination.Value;
            _webHostEnvironment = webHostEnvironment;
        }

        public void Initialize(Order order = null)
        {
            ViewData["Patients"] = _patientRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", "RegNo", order?.PatientId);
            ViewData["Doctors"] = _doctorRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", order?.DoctorId);
            var branch = _branchService.GetBranchByUser();
            ViewData["UseVoucherFormat"] = branch.UseVoucherFormatForOrder;
        }

        // GET
        [EnableCors]
        public IActionResult Index(int? page = 1, int? OrderId = null, int? PatientId = null, int? DoctorId = null, string VoucherNo = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            var user = _userService.Get(User);
            var orders = _orderRepository.GetAll(user.BranchId, user.OutletId, null, PatientId, DoctorId, VoucherNo, null, null, StartDate, EndDate, null, null, null);

            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            return View(orders.OrderBy(x =>x.IsPaid).ToList().ToPagedList((int)page, pageSize));
        }

        public IActionResult OrderReport(int? page = 1, int? OrderId = null, int? PatientId = null, int? DoctorId = null, string VoucherNo = null, DateTime? FromDate = null, DateTime? ToDate = null,int? OutletId=null)
        {
            var user = _userService.Get(User);
            if(FromDate==null)
            {
                FromDate = DateTime.Now.Date;
                ToDate = DateTime.Now.Date;
            }            
            var orders = _orderRepository.GetAll(user.BranchId, OutletId, null, null, null, null, true, null, FromDate, ToDate, null, null, null);
            var outlets = _outletRepository.GetAll(_branchService.GetBranchIdByUser());
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["Outlets"] = new SelectList(outlets, "Id", "Name");
            TempData["Page"] = page;
            TempData["FromDate"] = FromDate;
            TempData["ToDate"] = ToDate;
            TempData["OutletId"] = OutletId;
            return View(orders.OrderBy(x => x.IsPaid).ToList().ToPagedList((int)page, pageSize));
        }


        public IActionResult Create()
        {
            Initialize();
            var order = new Order
            {
                VoucherNo = _branchService.GetVoucherNo(VoucherTypeEnum.Pharmacy)
            };
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (order.OrderItems != null && order.OrderItems.Count() > 0)
                        {
                            var branch = _branchService.GetBranchByUser();
                            order.VoucherNo = _branchService.GetVoucherNo(VoucherTypeEnum.Pharmacy, order.VoucherNo);
                            if (string.IsNullOrEmpty(order.VoucherNo))
                            {
                                ModelState.AddModelError("VoucherNo", "This field is required.");
                                Initialize(order);
                                return View(order);
                            }
                            if (branch.AutoPaidForOrder)
                            {
                                order.IsPaid = true;
                                order.PaidDate = DateTime.Now;
                            }
                            order.Total = order.OrderItems.CalculateTotal() + order.OrderServices.CalculateTotal() + order.Tax - order.Discount; //_orderRepository.CalculateTotal(order.OrderItems.ToList());
                            order = await _userManager.AddUserAndTimestamp(order, User, DbEnum.DbActionEnum.Create);
                            order.OutletId = (int)_userService.Get(User).OutletId;
                            var _order = await _orderRepository.AddAsync(order);
                            if (_order != null)
                            {
                                await _outletRepository.UpdateStockAsync(order.OrderItems.GetOutletItemForUpdate(_orderRepository.GetOrderItemsForUpdate(_order.Id, _order.OutletId), _order.OutletId), (int)DbActionEnum.Delete);
                                await _branchService.IncreaseVoucherNo(VoucherTypeEnum.Pharmacy);
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
            Initialize(order);
            return View(order);
        }

        public IActionResult Edit(int id)
        {
            var order = _orderRepository.Get(id);
            Initialize(order);
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Order order)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (order.OrderItems != null && order.OrderItems.Count() > 0)
                        {
                            order.Total = order.OrderItems.CalculateTotal() + order.OrderServices.CalculateTotal() + order.Tax - order.Discount; //_orderRepository.CalculateTotal(order.OrderItems.ToList());
                            order = await _userManager.AddUserAndTimestamp(order, User, DbEnum.DbActionEnum.Update);
                            order.OutletId = (int)_userService.Get(User).OutletId;
                            var _order = await _orderRepository.UpdateAsync(order);
                            if (_order != null)
                            {
                                await _outletRepository.UpdateStockAsync(order.OrderItems.GetOutletItemForUpdate(_orderRepository.GetOrderItemsForUpdate(_order.Id, _order.OutletId), _order.OutletId), (int)DbActionEnum.Delete);
                                await transaction.CommitAsync();
                                TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {
                            ViewData["Error"] = "Order Items are required";
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.InnerException.Message);
                    await transaction.RollbackAsync();
                }
            }
            Initialize(order);
            return View(order);
        }

        public async Task<IActionResult> Delete(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var order = _orderRepository.Get(id);
                    var outletItemsForDelete = order.OrderItems.GetOutletItemForDelete(_orderRepository.GetOrderItemsForUpdate(order.Id, order.OutletId), order.OutletId);
                    var isSucceed = await _orderRepository.DeleteAsync(id);
                    if (isSucceed)
                    {
                        await _outletRepository.UpdateStockAsync(outletItemsForDelete, (int)DbActionEnum.Delete);
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
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                order.IsPaid = true;
                order.PaidDate = DateTime.Now;
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
            string patientName = "";
            using (var report = new LocalReport())
            {
                var order = _orderRepository.Get(id);
                var orderItems = order.OrderItems.Select(x => new
                {
                    Item = x.ItemName,
                    UnitPrice = x.UnitPrice.ToString("0.00"),
                    Qty = x.Qty,
                    Price = (x.UnitPrice * x.Qty).ToString("0.00"),
                    VoucherNo = order.VoucherNo,
                    Date = order.Date.ToString("dd-MM-yyyy"),
                    IsPaid = order.IsPaid,
                    Tax = order.Tax.ToString("0.00"),
                    Discount = order.Discount.ToString("0.00"),
                    PatientName = order.PatientName,
                    BranchName = order.BranchName,
                    Address = order.Address,
                    Phone = order.Phone,
                    DoctorName = order.DoctorName,
                    ConsultantFee=order.Doctor.CFFee+order.Branch.ClinicFee
                    

                }).ToList();
                var orderServices = order.OrderServices.Select(x => new
                {
                    Item = x.ServiceName,
                    UnitPrice = x.UnitPrice.ToString("0.00"),
                    Qty = x.Qty,
                    Price = (x.UnitPrice * x.Qty).ToString("0.00"),
                    VoucherNo = order.VoucherNo,
                    Date = order.Date.ToString("dd-MM-yyyy"),
                    IsPaid = order.IsPaid,
                    Tax = order.Tax.ToString("0.00"),
                    Discount = order.Discount.ToString("0.00"),
                    PatientName = order.PatientName,
                    BranchName = order.BranchName,
                    Address = order.Address,
                    Phone = order.Phone,
                    DoctorName = order.DoctorName,
                    ConsultantFee = order.Doctor.CFFee + order.Branch.ClinicFee
                }).ToList();
                report.DataSources.Add(new ReportDataSource("dsPharmacyReceipt", orderItems.Union(orderServices)));
                report.ReportPath = $"{_webHostEnvironment.WebRootPath}\\ReportFiles\\PharmacyReceipt.rdlc";

                var pdf = report.Render(renderFormat);
                return File(pdf, mimetype, order.PatientName+"_Pharmacy_"+DateTime.Now+"." + extension);
            }
        }

        public IActionResult PrintSlipRDLC(int id)
        {
            string renderFormat = "PDF";
            string extension = "pdf";
            string mimetype = "application/pdf";
            using (var report = new LocalReport())
            {
                var order = _orderRepository.Get(id);               
                
                var orderItems = order.OrderItems.Select(x => new
                {
                    Item = x.ItemName,
                    UnitPrice = x.UnitPrice.ToString("0.00"),
                    Qty = x.Qty,
                    Price = (x.UnitPrice * x.Qty).ToString("0.00"),
                    VoucherNo = order.VoucherNo,
                    Date = order.Date.ToString("dd-MM-yyyy"),
                    IsPaid = order.IsPaid,
                    Tax = order.Tax.ToString("0.00"),
                    Discount = order.Discount.ToString("0.00"),
                    PatientName = order.PatientName,
                    BranchName = order.BranchName,
                    Address = order.Address,
                    Phone = order.Phone,
                    DoctorName = order.DoctorName,
                    UnitShortForm = x.ShortForm,
                    ConsultantFee=order.Doctor.CFFee+order.Branch.ClinicFee


                }).ToList();
                var orderServices = order.OrderServices.Select(x => new
                {
                    Item = x.ServiceName,
                    UnitPrice = x.UnitPrice.ToString("0.00"),
                    Qty = x.Qty,
                    Price = (x.UnitPrice * x.Qty).ToString("0.00"),
                    VoucherNo = order.VoucherNo,
                    Date = order.Date.ToString("dd-MM-yyyy"),
                    IsPaid = order.IsPaid,
                    Tax = order.Tax.ToString("0.00"),
                    Discount = order.Discount.ToString("0.00"),
                    PatientName = order.PatientName,
                    BranchName = order.BranchName,
                    Address = order.Address,
                    Phone = order.Phone,
                    DoctorName = order.DoctorName,
                    UnitShortForm = "",
                    ConsultantFee = order.Doctor.CFFee + order.Branch.ClinicFee

                }).ToList();
                report.DataSources.Add(new ReportDataSource("dsPharmacyReceipt", orderItems.Union(orderServices)));
                report.ReportPath = $"{_webHostEnvironment.WebRootPath}\\ReportFiles\\PharmacyReceipt80mm.rdlc";

                var pdf = report.Render(renderFormat);
                return File(pdf, mimetype, order.PatientName + "_Pharmacy_" + DateTime.Now + "." + extension);
            }
        }

        public IActionResult DownloadReport()
        {
            var page = TempData["Page"];

            DateTime? fromDate = Convert.ToDateTime(TempData["FromDate"]);
            DateTime? toDate = Convert.ToDateTime(TempData["ToDate"]);
            int? outletId=null;
            if (TempData["FromDate"] == null)
            {
                fromDate = DateTime.Now.Date;
                toDate = DateTime.Now.Date;

            }
            if(TempData["OutletId"]!=null)
            {
                outletId = (int)TempData["OutletId"];
            }
            string renderFormat = "PDF";
            string extension = "pdf";
            string mimetype = "application/pdf";
            using (var report = new LocalReport())
            {
                List<Order> Orders = new List<Order>();
                List<Branch> branches = new List<Branch>();
                decimal ReferrerFee = 0;
                var user = _userService.Get(User);
                Orders = _orderRepository.GetAll(user.BranchId, outletId, null, null, null, null, true, null, fromDate, toDate, null, null, null);
                foreach(var o in Orders)
                {                 
                    ReferrerFee += o.ReferrerFee!=null?Convert.ToDecimal(o.ReferrerFee):0;
                }
                var branch = _branchService.GetBranchById((int)user.BranchId);
                branches.Add(branch);
                ReportParameter[] parameters = new ReportParameter[3];
                parameters[0] = new ReportParameter("FromDate", fromDate.ToString());
                parameters[1] = new ReportParameter("ToDate", toDate.ToString());
                parameters[2] = new ReportParameter("ReferrerFee", ReferrerFee.ToString());
                report.DataSources.Add(new ReportDataSource("Branch", branches));
                report.DataSources.Add(new ReportDataSource("dsOrder", Orders));
               
                report.ReportPath = $"{_webHostEnvironment.WebRootPath}\\ReportFiles\\OrderReport.rdlc";
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat); 
                TempData["Page"] = page;
                TempData["FromDate"] = fromDate;
                TempData["ToDate"] = toDate;
                TempData["OutletId"] = outletId;
                return File(pdf, mimetype,   "Order_" + DateTime.Now + "." + extension);
            }
        }

        public IActionResult GetDataForPrintSlip(int id)
        {
            var order = _orderRepository.Get(id);

            Slip slip = new Slip();
            slip.Hospital = order.BranchName;
            slip.Address = order.Address;
            slip.Phone = order.Phone;
            slip.Doctor = order.DoctorName;
            slip.Patient = order.PatientName;
            slip.VoucherNo = order.VoucherNo;
            slip.Date = DateTime.Now.ToString("dd-MM-yyyy hh:mmtt");
            slip.SubTotal = string.Format("{0:#,###}", order.Total + order.Discount - order.Tax);
            slip.Tax = string.Format("{0:#,###}", order.Tax);
            slip.Discount = string.Format("{0:#,###}", order.Discount);
            slip.GrandTotal = string.Format("{0:#,###}", order.Total);
            slip.SubTotal = string.IsNullOrEmpty(slip.SubTotal) ? "0" : slip.SubTotal;
            slip.Tax = string.IsNullOrEmpty(slip.Tax) ? "0" : slip.Tax;
            slip.Discount = string.IsNullOrEmpty(slip.Discount) ? "0" : slip.Discount;
            slip.GrandTotal = string.IsNullOrEmpty(slip.GrandTotal) ? "0" : slip.GrandTotal;
            var orderItems = order.OrderItems.Select(x => new SlipItem(x.ItemName, x.Qty + " " + x.ShortForm, string.Format("{0:#,###}", x.UnitPrice * x.Qty))).ToList();
            var orderServices = order.OrderServices.Select(x => new SlipItem(x.ServiceName, x.Qty.ToString(), string.Format("{0:#,###}", x.UnitPrice * x.Qty))).ToList();
            orderServices.Add(new SlipItem("Consultant Fee", "", string.Format("{0:#,###}", (order.ConsultantFee+order.ClinicFee).ToString())));

            slip.SlipItems = orderItems.Union(orderServices).ToList();
            return Ok(slip);
        }
    }
}