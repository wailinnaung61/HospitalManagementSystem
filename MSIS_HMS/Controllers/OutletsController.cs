using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
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
using NLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using ClosedXML.Excel;
using System.IO;
using Microsoft.Reporting.NETCore;
using MSIS_HMS.Core.Entities.DTOs;
using Microsoft.AspNetCore.Hosting;

namespace MSIS_HMS.Controllers
{
    [Authorize]
    public class OutletsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IOutletRepository _outletRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IWarehouseService _warehouseService;
        private readonly IUserService _userService;
        private readonly IBranchService _branchService;
        private readonly ILogger<OutletsController> _logger;
        private readonly IItemService _itemService;
        private readonly Pagination _pagination;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public OutletsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IOutletRepository outletRepository, IWarehouseRepository warehouseRepository, IWarehouseService warehouseService, IUserService userService, IBranchService branchService,ILogger<OutletsController> logger,IOptions<Pagination> pagination,IItemService itemService,IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _context = context;
            _outletRepository = outletRepository;
            _warehouseRepository = warehouseRepository;
            _warehouseService = warehouseService;
            _branchService = branchService;
            _userService = userService;
            _logger = logger;
            _pagination = pagination.Value;
            _itemService = itemService;
            _webHostEnvironment = webHostEnvironment;
        }

        public void Initialize(Outlet outlet = null)
        {
            ViewData["Warehouse"] = _warehouseService.GetSelectListItems(outlet?.Id);
            ViewData["Branches"] = _branchService.GetSelectListItems(outlet?.BranchId);
        }

        public IActionResult Index(int? page = 1, string OutletName = null, string OutletCode = null, int? WarehouseId = null)
        {
            var outlets = _outletRepository.GetAll(_branchService.GetBranchIdByUser(),OutletName,OutletCode,WarehouseId);
            var branches = _branchService.GetAll();
            //var warehouses = _warehouseService.GetAll();
            outlets.ForEach(x => x.Branch = branches.SingleOrDefault(b => b.Id == x.BranchId));
            //outlets.ForEach(x => x.Warehouse = warehouses.SingleOrDefault(b => b.Id == x.WarehouseId));
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["Warehouse"] = _warehouseService.GetSelectListItems(WarehouseId);
            return View(outlets.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
            
            
        }

        public IActionResult Stock(int? page = 1, int? WarehouseId = null,int? OutletId=null, int? ItemId = null)
        {
            var warehouseItems = _outletRepository.GetOutetStocks(_branchService.GetBranchIdByUser(), WarehouseId,OutletId, ItemId);
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["Warehouses"] = _warehouseService.GetSelectListItems(WarehouseId);
            ViewData["Items"] = _itemService.GetSelectListItems(ItemId);
            return View(warehouseItems.ToPagedList((int)page, pageSize));
        }

        public IActionResult OutletStockReport(int? page = 1, int? WarehouseId = null, int? OutletId = null, int? ItemId = null)
        {
            var warehouseItems = _outletRepository.GetOutetStocks(_branchService.GetBranchIdByUser(), WarehouseId, OutletId, ItemId);
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["Warehouses"] = _warehouseService.GetSelectListItemsByBranch(_branchService.GetBranchIdByUser(), WarehouseId);
            ViewData["Items"] = _itemService.GetSelectListItems(ItemId);
            TempData["WarehouseId"] = WarehouseId;
            TempData["OutletId"] = OutletId;
            return View(warehouseItems.ToPagedList((int)page, pageSize));
        }

        public IActionResult Create()
        {
            Initialize();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Outlet outlet)
        {
            MappedDiagnosticsLogicalContext.Set("userId", _userManager.GetUserId(User));
            try
            {
                if (ModelState.IsValid)
                {
                    outlet = await _userManager.AddUserAndTimestamp(outlet, User, DbEnum.DbActionEnum.Create);
                    var _outlet = await _outletRepository.AddAsync(outlet);
                    if (_outlet != null)
                    {
                        TempData["notice"] = StatusEnum.NoticeStatus.Success;
                        _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }
            Initialize(outlet);
            return View();
        }
        public ActionResult Edit(int id)
        {
            Initialize();
            var locations = _outletRepository.Get(id);
            return View(locations);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Outlet outlet)
        {
            MappedDiagnosticsLogicalContext.Set("userId", _userManager.GetUserId(User));
            try
            {
                if (ModelState.IsValid)
                {
                    outlet = await _userManager.AddUserAndTimestamp(outlet, User, DbEnum.DbActionEnum.Update);
                    var _location = await _outletRepository.UpdateAsync(outlet);                   
                    TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                    _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }
            Initialize();
            return View(outlet);
        }
        public async Task<ActionResult> Delete(int id)
        {
            MappedDiagnosticsLogicalContext.Set("userId", _userManager.GetUserId(User));
            try
            {
                var _outlet = await _outletRepository.DeleteAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Delete));
              
            }
            catch(Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }
            
            return RedirectToAction(nameof(Index));
        }
        public IActionResult GetOutletItems(int outletId)
        {
            var outletItems = _outletRepository.GetItemsFromOutlet((int)_userService.Get(User).BranchId, outletId);
            return Ok(outletItems);
        }
        public IActionResult GetOutletItemsByUserOutlet()
        {
            var user = _userService.Get(User);
            if(user == null)
            {
                return Unauthorized();
            }
            if (user.OutletId != null)
            {
                var outletItems = _outletRepository.GetItemsFromOutlet((int)_branchService.GetBranchIdByUser(), (int)user.OutletId);
                return Ok(outletItems);
            }
            return BadRequest();
        }
        public IActionResult GetAll(int? BranchId)
        {
            var outletItems = _outletRepository.GetAll(BranchId ?? -1).OrderBy(x => x.Name);
            return Ok(outletItems);
        }

        public IActionResult GetAllOutlets()
        {
            var outletItems = _outletRepository.GetAll(_userService.Get(User).BranchId).OrderBy(x => x.Name);
            return Ok(outletItems);
        }

        public IActionResult GetOutletByWarehouseId(int? warehouseId)
        {
            var outlets = _outletRepository.GetAll(_branchService.GetBranchIdByUser(), null, null, warehouseId ?? -1, null).OrderBy(x => x.Name);
            return Ok(outlets);
        }

        [HttpGet]
        public IActionResult ExcelExport(int? WarehouseId = null, int? OutletId = null, int? ItemId = null)
        {
            var outletItems = _outletRepository.GetOutetStocks(_branchService.GetBranchIdByUser(), WarehouseId, OutletId, ItemId);
            // var result = _reportService.WarehouseItemByExRemindDayexcelExport(warehouseItems);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("OutletStock");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "Warhouse";
                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 2).Value = "Outlet";
                worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 3).Value = "Item(Code)";
                worksheet.Cell(currentRow, 3).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 4).Value = "Qty";
                worksheet.Cell(currentRow, 4).Style.Font.Bold = true;
               

                foreach (var w in outletItems)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = w.WarehouseName;
                    worksheet.Cell(currentRow, 2).Value = w.OutletName;
                    worksheet.Cell(currentRow,3).Value= w.ItemName + "(" + w.ItemCode + ")";
                    worksheet.Cell(currentRow, 4).Value = w.Qty;
                    
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Outlet.xlsx");
                }

            }

        }

        public IActionResult DownloadReport()
        {
            var page = TempData["Page"];
            int? warehouseId = null;
            int? outletId = null;
            //int? itemId = null;
            if (TempData["WarehouseId"] != null)
            {
                warehouseId = (int)TempData["WarehouseId"];
            }
            if (TempData["OutletId"] != null)
            {
                outletId = (int)TempData["OutletId"];
            }
           
            string renderFormat = "PDF";
            string extension = "pdf";
            string mimetype = "application/pdf";
            using (var report = new LocalReport())
            {
                var user = _userService.Get(User);
                List<OutletStockItemDTO> warehouseItems = new List<OutletStockItemDTO>();
                List<Branch> branches = new List<Branch>();
                var branch = _branchService.GetBranchById((int)user.BranchId);
                branches.Add(branch);
                // var user = _userService.Get(User);
                warehouseItems = _outletRepository.GetOutetStocks(_branchService.GetBranchIdByUser(), warehouseId, outletId, null);

                report.DataSources.Add(new ReportDataSource("Branch", branches));
                report.DataSources.Add(new ReportDataSource("dsOutletStock", warehouseItems));

                report.ReportPath = $"{_webHostEnvironment.WebRootPath}\\ReportFiles\\OutletStockReport.rdlc";
                var pdf = report.Render(renderFormat);
                TempData["Page"] = page;
                TempData["WarehouseId"] = warehouseId;
                TempData["OutletId"] = outletId;
              
                return File(pdf, mimetype, "OutletStockReport_" + DateTime.Now + "." + extension);
            }
        }
    }
}
