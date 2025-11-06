using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Helpers;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Interfaces;
using MSIS_HMS.Models;
using System.Drawing;

namespace MSIS_HMS.Controllers
{

    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILabOrderRepository _labOrderRepository;
        private readonly IVisitRepository _visitRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUserService _userService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(UserManager<ApplicationUser> userManager,ApplicationDbContext context,ILabOrderRepository labOrderRepository,IUserService userService ,ILogger<HomeController> logger,IVisitRepository visitRepository,IOrderRepository orderRepository,IPurchaseRepository purchaseRepository,IDoctorRepository doctorRepository)
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
                
        }

        public IActionResult Index()
        {
           
            var user = _userService.Get(User);
            if(user.UserName== "superadmin@gmail.com")
            {
                return RedirectToAction("Index", "Modules");
            }
            else
            {
                var userInfo = _context.Users.Where(x => x.Id == user.Id).FirstOrDefault();

                var accessMenus = _context.UserAccessMenus.Include(x => x.Menu).Where(x => x.UserId == userInfo.Id && x.Menu.IsDashboard == true).FirstOrDefault();
                return RedirectToAction(accessMenus.Menu.Action, accessMenus.Menu.Controller);
            }
            
            //decimal amtlabOrder = 0;
            //decimal amtOrder = 0;
            //decimal amtPurchase = 0;
            ////var labOrders = _labOrderRepository.GetAll(user.BranchId, null, null, null, null, null, DateTime.Now.Date, DateTime.Now.Date, null, null, null);
            //var labOrders = _labOrderRepository.GetAll(user.BranchId, null, null, null, null, null, null, null, null, null, null);
            //foreach (var laborder in labOrders)
            //{
            //    amtlabOrder += laborder.Total;
            //}
            //var orders = _orderRepository.GetAll(user.BranchId, null, null, null, null, null, null, null, null, null, null, null, null);
            //foreach(var order in orders)
            //{
            //    amtOrder += order.Total;
            //}
            //var purchases= _purchaseRepository.GetAll(user.BranchId, null, null, null, null, null, null, null, null);
            //foreach(var purchase in purchases)
            //{
            //    amtPurchase += purchase.Total;
            //}
            //var visits = _visitRepository.GetVisitPatient(_userService.Get(User).BranchId, DateTime.Now.Date,DateTime.Now.Date,Core.Enums.VisitStatusEnum.Booked).ToList();
            //var doctors = _doctorRepository.GetAll(_userService.Get(User).BranchId, null, null, null, null, null, null);
            //ViewData["Doctors"] = doctors;
            //ViewData["VisitPatient"] = visits;
            //ViewData["LabOrder"] = labOrders.Count;
            //ViewData["LabOrderAmt"] = amtlabOrder.FormatMoney();
            //ViewData["OrderAmt"] = amtOrder.FormatMoney();
            //ViewData["PurchaseAmt"] = amtPurchase.FormatMoney();
           // return View();
        }

        public IActionResult GetVisitCount()
        {
            List<VisitDashboard> visitDashboards = new List<VisitDashboard>();
           for(int i=1;i<=12;i++)
            {
                var visitCount = _context.Visits.Where(x => x.Date.Year == DateTime.Now.Year && x.Date.Month == i && x.IsDelete == false).ToList();
                VisitDashboard visitDashboard = new VisitDashboard();
                visitDashboard.count = visitCount.Count;
                visitDashboard.month = i;// new DateTime(DateTime.Now.Year, i, 1).ToString("MMM", CultureInfo.InvariantCulture);
                visitDashboards.Add(visitDashboard);
            }
            return Json(visitDashboards);
        }
        public IActionResult GetPharmacyOrdercount()
        {
            List<OrderDashboard> orderDashboards = new List<OrderDashboard>();
            for(int i=1;i<=DateTime.Now.Month;i++)
            {
                var orderCount = _context.Orders.Where(x => x.Date.Year == DateTime.Now.Year && x.Date.Month == i && x.IsDelete == false).ToList();
                OrderDashboard orderDashboard = new OrderDashboard();
                orderDashboard.count = orderCount.Count;
                orderDashboard.month = new DateTime(DateTime.Now.Year, i, 1).ToString("MMM", CultureInfo.InvariantCulture);
                orderDashboards.Add(orderDashboard);
            }
            return Json(orderDashboards);
        }
      
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
