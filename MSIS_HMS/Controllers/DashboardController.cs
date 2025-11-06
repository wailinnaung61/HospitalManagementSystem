using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Helpers;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Interfaces;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MSIS_HMS.Controllers
{
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILabOrderRepository _labOrderRepository;
        private readonly IVisitRepository _visitRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUserService _userService;
        private readonly IPatientRepository _patientRepository;
        private readonly IIPDRecordRepository _iPDRecordRepository;
        private readonly IOperationTreaterRepository _operationTreaterRepository;
        private readonly IItemRepository _itemRepository;
        private readonly ILabPersonRepository _labPersonRepository;
        private readonly ILabTestRepository _labTestRepository;
        private readonly ILabResultRepository _labResultRepository;
        private readonly IDeliverOrderRepository _deliverOrderRepository;
        private readonly IWarehouseTransferRepository _warehouseTransferRepository;
        private readonly IOutletTransferRepository _outletTransferRepository;
        private readonly IReturnRepository _returnRepository;
        private readonly IScratchRepository _scratchRepository;
        private readonly IItemTypeRepository _itemTypeRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IOutletRepository _outletRepository;
        private readonly IBatchRepository _batchRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly ILogger<HomeController> _logger;
        private readonly IOperationTypeRepository _operationTypeRepository;

        public DashboardController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILabOrderRepository labOrderRepository, IUserService userService, ILogger<HomeController> logger, IVisitRepository visitRepository, IOrderRepository orderRepository, IPurchaseRepository purchaseRepository, IDoctorRepository doctorRepository,IPatientRepository patientRepository,IIPDRecordRepository iPDRecordRepository,IOperationTreaterRepository operationTreaterRepository,IItemRepository itemRepository,ILabPersonRepository labPersonRepository,ILabTestRepository labTestRepository,ILabResultRepository labResultRepository,IDeliverOrderRepository deliverOrderRepository,IWarehouseTransferRepository warehouseTransferRepository,IOutletTransferRepository outletTransferRepository,IReturnRepository returnRepository,IScratchRepository scratchRepository,IItemTypeRepository itemTypeRepository,IWarehouseRepository warehouseRepository,IOutletRepository outletRepository,IBatchRepository batchRepository,ILocationRepository locationRepository,IStaffRepository staffRepository,IOperationTypeRepository operationTypeRepository)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _labOrderRepository = labOrderRepository;
            _userService = userService;
            _visitRepository = visitRepository;
            _orderRepository = orderRepository;
            _purchaseRepository = purchaseRepository;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _iPDRecordRepository = iPDRecordRepository;
            _operationTreaterRepository = operationTreaterRepository;
            _itemRepository = itemRepository;
            _labPersonRepository = labPersonRepository;
            _labTestRepository = labTestRepository;
            _labResultRepository = labResultRepository;
            _deliverOrderRepository = deliverOrderRepository;
            _warehouseTransferRepository = warehouseTransferRepository;
            _outletTransferRepository = outletTransferRepository;
            _returnRepository = returnRepository;
            _scratchRepository = scratchRepository;
            _itemTypeRepository = itemTypeRepository;
            _warehouseRepository = warehouseRepository;
            _outletRepository = outletRepository;
            _batchRepository = batchRepository;
            _locationRepository = locationRepository;
            _staffRepository = staffRepository;
            _operationTypeRepository = operationTypeRepository;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            var user = _userService.Get(User);
            decimal totalDailyAmt = 0;
            decimal totalMonthlyAmt = 0;
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var DailyIncome = _orderRepository.GetIncome(user.BranchId,  DateTime.Now.Date, DateTime.Now.Date);
            var MonthlyIncome = _orderRepository.GetIncome(user.BranchId, startDate, endDate);
            var TotalPatient = _patientRepository.GetAll(user.BranchId);
            var TodayOrder = _orderRepository.GetAll(user.BranchId, null, null, null, null, null, null, DateTime.Now.Date, DateTime.Now.Date);
            var TodayVisit = _visitRepository.GetAll(user.BranchId, null, null, null, DateTime.Now.Date, DateTime.Now.Date, null, null, null);
            var TodayAdmitted = _iPDRecordRepository.GetAll(user.BranchId, null, null, null, null, null, null, null, DateTime.Now.Date,null);
            var TodayDischarged=_iPDRecordRepository.GetAll(user.BranchId, null, null, null, null, null, null, null, null, DateTime.Now.Date);
            var TodayOT = _operationTreaterRepository.GetAll(user.BranchId, null, null, null, null, null, null, null, DateTime.Now.Date, DateTime.Now.Date, null, null, null);
            var DailyAndMonthlyIncome = _orderRepository.GetIncomeForDailyandMonthly(user.BranchId,null, null);
            for (int i = 0; i < DailyAndMonthlyIncome.Count; i++)
            {
                DailyAndMonthlyIncome[i].No = i + 1;
                totalDailyAmt += DailyAndMonthlyIncome[i].DailyIncome;
                totalMonthlyAmt += DailyAndMonthlyIncome[i].MonthlyIncome;
            }
            
            ViewData["DailyIncome"] = DailyIncome.FormatMoney();
            ViewData["MonthlyIncome"] = MonthlyIncome.FormatMoney();
            ViewData["TotalPatient"] = TotalPatient.Count;
            ViewData["TodayOrder"] = TodayOrder.Count;
            ViewData["TodayVisit"] = TodayVisit.Count;
            ViewData["TodayAdmitted"] = TodayAdmitted.Count;
            ViewData["TodayDischarged"] = TodayDischarged.Count;
            ViewData["TodayOT"] = TodayOT.Count;
            ViewData["DailyAndMonthly"] = DailyAndMonthlyIncome;
            ViewData["TotalDailyAmt"] = totalDailyAmt;
            ViewData["TotalMonthlyAmt"] = totalMonthlyAmt;
            DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ViewData["CurrentMonth"] = date.ToString("MMM");

            return View();
        }
        public IActionResult GetMonthlyIncome()
        {
            var user = _userService.Get(User);
            DateTime now = DateTime.Now;
           
            List<MonthlyIncomeDTO> orderDashboards = new List<MonthlyIncomeDTO>();
            for (int i = 1; i <= DateTime.Now.Month; i++)
            {
                var startDate = new DateTime(now.Year, i, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                var MonthlyIncome = _orderRepository.GetIncome(user.BranchId, startDate, endDate);
                MonthlyIncomeDTO monthlyIncomeDTO = new MonthlyIncomeDTO();
                monthlyIncomeDTO.Amount = MonthlyIncome;
                monthlyIncomeDTO.Month = new DateTime(DateTime.Now.Year, i, 1).ToString("MMM", CultureInfo.InvariantCulture);
                orderDashboards.Add(monthlyIncomeDTO);
            }
            return Json(orderDashboards);
        }
        public IActionResult Lab()
        {
            var user = _userService.Get(User);
            decimal serviceFee = 0;
            var DailyIncome = _labOrderRepository.GetIncomeForLabDashboard(user.BranchId);
            var TodayLabOrder = _labOrderRepository.GetAll(user.BranchId, null, null, null, null, DateTime.Now.Date, DateTime.Now.Date, null, null, null, null);
            var CompleteLab = _labOrderRepository.GetLabOrderResultComplete(user.BranchId);
            var TotalTechnician = _labPersonRepository.GetAll(user.BranchId, null, null, null, Core.Enums.LabPersonTypeEnum.Technician, null, null);
            var TotalConsultant=_labPersonRepository.GetAll(user.BranchId, null, null, null, Core.Enums.LabPersonTypeEnum.Consultant, null, null);
            var TotalTest = _labTestRepository.GetAll(user.BranchId, null, null, null, null, null);
            var labResults = _labResultRepository.GetAll(user.BranchId, null, null, null, null, null, null, null, DateTime.Now.Date, DateTime.Now.Date, null, null, null, null); ;
            foreach (var lab in labResults)
            {
                serviceFee += CalculationExtensions.GetFee(lab.UnitPrice, lab.TechnicianFee, lab.TechnicianFeeType);
                serviceFee+= CalculationExtensions.GetFee(lab.UnitPrice, lab.ConsultantFee, lab.ConsultantFeeType);
            }
            ViewData["DailyIncome"] = DailyIncome[0].DailyIncome.FormatMoney();
            ViewData["MonthlyIncome"] = DailyIncome[0].MonthlyIncome.FormatMoney();
            ViewData["TodayLabOrder"] = TodayLabOrder.Count;
            ViewData["CompleteLab"] = CompleteLab.Count;
            ViewData["TotalTechnician"] = TotalTechnician.Count;
            ViewData["TotalConsultant"] = TotalConsultant.Count;
            ViewData["TotalTest"] = TotalTest.Count;
            ViewData["ServiceFee"] = serviceFee;

            ViewData["LabResults"] = _context.LabResults.Include(x => x.Patient).Include(x => x.LabTest).Include(x => x.Technician).Include(x => x.Consultant).Where(x => !x.IsDelete && !x.IsCompleted).OrderBy(x => x.Date).ToList();
            return View();
        }
        public IActionResult GetLabOrdercount()
        {
            List<OrderDashboard> orderDashboards = new List<OrderDashboard>();
            for (int i = 1; i <= DateTime.Now.Month; i++)
            {
                var orderCount = _context.LabOrders.Where(x => x.Date.Year == DateTime.Now.Year && x.Date.Month == i && x.IsDelete == false).ToList();
                OrderDashboard orderDashboard = new OrderDashboard();
                orderDashboard.count = orderCount.Count;
                orderDashboard.month = new DateTime(DateTime.Now.Year, i, 1).ToString("MMM", CultureInfo.InvariantCulture);
                orderDashboards.Add(orderDashboard);
            }
            return Json(orderDashboards);
        }

        public IActionResult Pharmacy()
        {
            var user = _userService.Get(User);
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var Income = _orderRepository.GetIncomeForPharmacyDashboard(user.BranchId);
            var TodayOrder = _orderRepository.GetAll(user.BranchId, null, null, null, null, null, null, DateTime.Now.Date, DateTime.Now.Date);
            var TotalOutlet = _context.Outlets.Where(x => x.BranchId == user.BranchId && x.IsDelete == false).ToList();
            var outletIncome = _orderRepository.GetOutletIncomeForPharmacyDashboard(user.BranchId);
            var outletItemformonth = _itemRepository.GetSaleItem(user.BranchId, startDate, endDate, null);
            int i = 0;
            foreach(var noval in outletItemformonth)
            {
                noval.No = ++i;
            }
            ViewData["DailyOrderIncome"] = Income[0].DailyIncome.FormatMoney();
            ViewData["MonthlyOrderIncome"] = Income[0].MonthlyIncome.FormatMoney();
            ViewData["TodayOrder"] = TodayOrder.Count;
            ViewData["TotalOutlet"] = TotalOutlet.Count;
            ViewData["OutletIncome"] = outletIncome;
            ViewData["OutletItemFormonth"] = outletItemformonth;

            return View();
        }
        public IActionResult GetPharmacyOrdercount()
        {
            List<OrderDashboard> orderDashboards = new List<OrderDashboard>();
            for (int i = 1; i <= DateTime.Now.Month; i++)
            {
                var orderCount = _context.Orders.Where(x => x.Date.Year == DateTime.Now.Year && x.Date.Month == i && x.IsDelete == false).ToList();
                OrderDashboard orderDashboard = new OrderDashboard();
                orderDashboard.count = orderCount.Count;
                orderDashboard.month = new DateTime(DateTime.Now.Year, i, 1).ToString("MMM", CultureInfo.InvariantCulture);
                orderDashboards.Add(orderDashboard);
            }
            return Json(orderDashboards);
        }

        public IActionResult OPD()
        {
            var user = _userService.Get(User);
            var visits = _visitRepository.GetVisitPatient(_userService.Get(User).BranchId, DateTime.Now.Date, DateTime.Now.Date, Core.Enums.VisitStatusEnum.Booked).ToList();
            var visitsCount = _visitRepository.GetVisitPatient(_userService.Get(User).BranchId, DateTime.Now.Date, DateTime.Now.Date, null).ToList();

            var doctors = _doctorRepository.GetAll(_userService.Get(User).BranchId, null, null, null, null, null, null);
            var newPatients = _patientRepository.GetAll(user.BranchId, null, DateTime.Now.Date, DateTime.Now.Date, null, null, null, null, null, null, null, null);
            var fees = _visitRepository.GetCFFee(user.BranchId, DateTime.Now.Date, DateTime.Now.Date);
            var cfFee = fees.Count > 0 ? Convert.ToDecimal(fees[0]) : 0;
           // var roundFee = Convert.ToDecimal(fees[1]);
            var doctorHistory = _visitRepository.GetDoctorHistory(_userService.Get(User).BranchId, DateTime.Now.Date, DateTime.Now.Date, null).ToList();
            

            ViewData["Doctors"] = doctors;
            ViewData["DoctorCount"] = doctors.Count;
            ViewData["VisitPatient"] = visits;
            ViewData["Visit"] = visitsCount.Count;
            ViewData["newPatients"] = newPatients.Count;
            ViewData["cfFee"] = cfFee.FormatMoney();
          //  ViewData["roundFee"] = roundFee.FormatMoney();
            ViewData["doctorHistory"] = doctorHistory;
            return View();
        }

        public IActionResult Inventory()
        {
            var user = _userService.Get(User);
            var purchaseOrder = _purchaseRepository.GetAll(user.BranchId, null, null, null, null, null, null, DateTime.Now.Date, DateTime.Now.Date);
            var deliverOrder = _deliverOrderRepository.GetAll(user.BranchId, null, null, null, null, null, null, DateTime.Now.Date, DateTime.Now.Date);
            var warehouseTransfer = _warehouseTransferRepository.GetAll(user.BranchId, null, null, null, null, null, null, DateTime.Now.Date, DateTime.Now.Date);
            var outletTransfer = _outletTransferRepository.GetAll(user.BranchId, null, null, DateTime.Now.Date, DateTime.Now.Date);
            var returned = _returnRepository.GetAll(user.BranchId, null, null, DateTime.Now.Date, DateTime.Now.Date);
            var scratched = _scratchRepository.GetAll(user.BranchId, null, null, null, DateTime.Now.Date, DateTime.Now.Date);
            var items = _itemRepository.GetAll(user.BranchId, null, null, null, null, null);
            var itemType = _itemTypeRepository.GetAll(user.BranchId);
            var warehouses = _warehouseRepository.GetAll(user.BranchId);
            var outlets = _outletRepository.GetAll(user.BranchId);
            var batches = _batchRepository.GetAll(user.BranchId);
            var locations = _locationRepository.GetAll(user.BranchId);
            var warehoueStock= _warehouseRepository.GetWarehouseItemReport(user.BranchId, null, null, null, null, null, null);
            var expirationItem = warehoueStock.Where(x => x.NearExpiry == true);
            ViewData["PurchaseOrder"] = purchaseOrder.Count;
            ViewData["DeliverOrder"] = deliverOrder.Count;
            ViewData["WarehouseTransfer"] = warehouseTransfer.Count;
            ViewData["OutletTransfer"] = outletTransfer.Count;
            ViewData["Returned"] = returned.Count;
            ViewData["Scratched"] = scratched.Count;
            ViewData["Items"] = items.Count;
            ViewData["ItemType"] = itemType.Count;
            ViewData["Warehouse"] = warehouses.Count;
            ViewData["Outlets"] = outlets.Count;
            ViewData["Batches"] = batches.Count;
            ViewData["Locations"]=locations.Count;
            ViewData["ExpiryItem"] = expirationItem;

            return View();
        }

        public IActionResult GetMonthlyPurchaseOrder()
        {
            List<OrderDashboard> orderDashboards = new List<OrderDashboard>();
            for (int i = 1; i <= DateTime.Now.Month; i++)
            {
                var orderCount = _context.Purchases.Where(x => x.PurchaseDate.Year == DateTime.Now.Year && x.PurchaseDate.Month == i && x.IsDelete == false).ToList();
                OrderDashboard orderDashboard = new OrderDashboard();
                orderDashboard.count = orderCount.Count;
                orderDashboard.month = new DateTime(DateTime.Now.Year, i, 1).ToString("MMM", CultureInfo.InvariantCulture);
                orderDashboards.Add(orderDashboard);
            }
            return Json(orderDashboards);
        }

        public IActionResult IPD()
        {
            var user = _userService.Get(User);
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var newPatient = _patientRepository.GetAll(user.BranchId, null, DateTime.Now.Date, DateTime.Now.Date, null, null, null, null, null, null, null, null);
            var admitted = _iPDRecordRepository.GetAll(user.BranchId, null, null, null, null, null, null, null, DateTime.Now.Date, null);
            var Discharged = _iPDRecordRepository.GetAll(user.BranchId, null, null, null, null, null, null, null, null, DateTime.Now.Date);
            var Doctors = _doctorRepository.GetAll(user.BranchId, null, null, null, null, null, null, null);
            var DailyIncome = _iPDRecordRepository.GetIncomeForIPD(user.BranchId, DateTime.Now.Date, DateTime.Now.Date);
            var MonthlyIncome = _iPDRecordRepository.GetIncomeForIPD(user.BranchId,startDate, endDate);
            var Floors = _context.Floors.Where(x => x.BranchId == user.BranchId && x.IsDelete == false).ToList();
            var Wards = _context.Wards.Include(x => x.Floor).Where(x => x.IsDelete == false && x.Floor.BranchId==user.BranchId).ToList();
            var Rooms = _context.Rooms.Include(x => x.Ward).Where(x => x.IsDelete == false && x.Ward.Floor.BranchId == user.BranchId).ToList();
            var Beds = _context.Beds.Include(x => x.Room).Where(x => x.IsDelete == false && x.Room.Ward.Floor.BranchId == user.BranchId).ToList();
            var BedTypes = _context.BedTypes.Where(x => x.IsDelete == false).ToList();
            var RoomTypes = _context.RoomTypes.Where(x => x.IsDelete == false).ToList();
            var Foods = _context.Foods.Where(x => x.IsDelete == false).ToList();
            var FoodCategory = _context.FoodCategories.Where(x => x.IsDelete == false).ToList();
            var admittedPatient = _iPDRecordRepository.GetIPDRecordForDashboard(user.BranchId);
            var fees = _visitRepository.GetCFFee(user.BranchId, DateTime.Now.Date, DateTime.Now.Date);           
            var roundFee=fees.Count>1? Convert.ToDecimal(fees[1]):0;

            ViewData["NewPatient"] = newPatient.Count;
            ViewData["Admitted"] = admitted.Count;
            ViewData["Discharged"] = Discharged.Count;
            ViewData["Doctors"] = Doctors.Count;
            ViewData["DailyIncome"] = DailyIncome.FormatMoney();
            ViewData["MonthlyIncome"] = MonthlyIncome.FormatMoney();
            ViewData["Floors"] = Floors.Count;
            ViewData["Wards"] = Wards.Count;
            ViewData["Rooms"] = Rooms.Count;
            ViewData["Beds"] = Beds.Count;
            ViewData["BedTypes"] = BedTypes.Count;
            ViewData["RoomTypes"] = RoomTypes.Count;
            ViewData["Foods"] = Foods.Count;
            ViewData["FoodCategory"] = FoodCategory.Count;
            ViewData["AdmittedPatient"] = admittedPatient;
            ViewData["RoundFee"] = roundFee.FormatMoney();


            return View();
        }
        public IActionResult GetMonthlyAdmittedPatient()
        {
            var user = _userService.Get(User);
            DateTime now = DateTime.Now;

            List<OrderDashboard> orderDashboards = new List<OrderDashboard>();
            for (int i = 1; i <= DateTime.Now.Month; i++)
            {
                var startDate = new DateTime(now.Year, i, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                var MonthlyAdmitted = _iPDRecordRepository.GetAll(user.BranchId,null,null,null,null,null,null,null,null,null,startDate,endDate);
                OrderDashboard orderDashboard = new OrderDashboard();
                orderDashboard.count = MonthlyAdmitted.Count;
                orderDashboard.month = new DateTime(DateTime.Now.Year, i, 1).ToString("MMM", CultureInfo.InvariantCulture);
                orderDashboards.Add(orderDashboard);
            }
            return Json(orderDashboards);
        }
        public IActionResult OperationThreater()
        {
            var user = _userService.Get(User);
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var DailyIncome = _operationTreaterRepository.GetIncomeForOT(user.BranchId, DateTime.Now.Date, DateTime.Now.Date);
            var MonthlyIncome = _operationTreaterRepository.GetIncomeForOT(user.BranchId, startDate, endDate);
            var operationToday = _operationTreaterRepository.GetAll(user.BranchId, null, null, null, null, null, null, null, DateTime.Now.Date, DateTime.Now.Date, null, null, null);
            var doctors= _doctorRepository.GetAll(user.BranchId, null, null, null, null, null, null, null);
            var nurses = _context.Staffs.Where(x => x.BranchId == user.BranchId && x.Position.Name == "Nurse").ToList();
            var operatoinQueue = _operationTreaterRepository.GetAll(user.BranchId, null, null, null, null, null, false, null, DateTime.Now.Date, DateTime.Now.Date, null, null, null);
            var operationName = _operationTypeRepository.GetAll(user.BranchId);

            ViewData["DailyIncome"] = DailyIncome.FormatMoney();
            ViewData["MonthlyIncome"] = MonthlyIncome.FormatMoney();
            ViewData["OperationToday"] = operationToday.Count;
            ViewData["Doctors"] = doctors.Count;
            ViewData["Nurses"] = nurses.Count;
            ViewData["OperationQueue"] = operatoinQueue;
            ViewData["OperationName"] = operationName.Count;
            return View();
        }
        public IActionResult GetMonthlyOT()
        {
            var user = _userService.Get(User);
            DateTime now = DateTime.Now;

            List<OrderDashboard> orderDashboards = new List<OrderDashboard>();
            for (int i = 1; i <= DateTime.Now.Month; i++)
            {
                var startDate = new DateTime(now.Year, i, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                var MonthlyAdmitted = _operationTreaterRepository.GetAll(user.BranchId,null,null,null,null,null,null,null,startDate,endDate,null,null,null);
                OrderDashboard orderDashboard = new OrderDashboard();
                orderDashboard.count = MonthlyAdmitted.Count;
                orderDashboard.month = new DateTime(DateTime.Now.Year, i, 1).ToString("MMM", CultureInfo.InvariantCulture);
                orderDashboards.Add(orderDashboard);
            }
            return Json(orderDashboards);
        }
    }
}
