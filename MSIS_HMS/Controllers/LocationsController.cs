using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using Microsoft.Extensions.Options;
using NLog;

namespace MSIS_HMS.Controllers
{
   
    public class LocationsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILocationRepository _locationRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IWarehouseService _warehouseService;
        private readonly IBranchService _branchService;
        private readonly ILogger<LocationsController> _logger;
        private readonly Pagination _pagination;
       
        public LocationsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context,ILocationRepository locationRepository,IWarehouseRepository warehouseRepository,IWarehouseService warehouseService,IBranchService branchService,ILogger<LocationsController> logger,IOptions<Pagination> pagination )
        {
            _userManager = userManager;
            _context = context;
            _locationRepository = locationRepository;
            _warehouseRepository = warehouseRepository;
            _warehouseService = warehouseService;
            _branchService = branchService;
            _logger = logger;
            _pagination = pagination.Value;
        }
        public void Initialize(Location location=null)
        {
            ViewData["Warehouse"] = _warehouseService.GetSelectListItems(location?.WarehouseId);
            ViewData["Branches"] = _branchService.GetSelectListItems(location?.BranchId);
        }
        public IActionResult Index(int? page = 1, string LocationName = null, string LocationCode = null, int? WarehouseId = null)
        {
            var locations = _locationRepository.GetAll(_branchService.GetBranchIdByUser(), LocationName, LocationCode,WarehouseId);
            var warehouses = _warehouseService.GetAll();
            locations.ForEach(x => x.Warehouse = warehouses.SingleOrDefault(i => i.Id == x.WarehouseId));
            var pageSize = _pagination.PageSize;
         ;   ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["Warehouse"] = _warehouseService.GetSelectListItems(WarehouseId);
            return View(locations.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));

        }
        public IActionResult Create()
        {
            Initialize();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Location location)
        {
            MappedDiagnosticsLogicalContext.Set("userId", _userManager.GetUserId(User));//insert userId into Logs Table
            try
            {
                ModelState.Remove("BranchId");
                if (ModelState.IsValid)
                {
                    location = await _userManager.AddUserAndTimestamp(location, User, DbEnum.DbActionEnum.Create);
                    var _locaiton = await _locationRepository.AddAsync(location);
                    if (_locaiton != null)
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
            Initialize(location);
            return View();
        }

        public ActionResult Edit(int id)
        {
            Initialize();
            var locations = _locationRepository.Get(id);
            return View(locations);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Location location)
        {
            MappedDiagnosticsLogicalContext.Set("userId", _userManager.GetUserId(User));
            try
            {
                if (ModelState.IsValid)
                {
                    location = await _userManager.AddUserAndTimestamp(location, User, DbEnum.DbActionEnum.Update);
                    var _location = await _locationRepository.UpdateAsync(location);                   
                    TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                    _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                Console.WriteLine(e.Message);
            }
            Initialize();
            return View(location);
        }
        public async Task<ActionResult> Delete(int id)
        {
            MappedDiagnosticsLogicalContext.Set("userId", _userManager.GetUserId(User));
            try
            {
                var _location = await _locationRepository.DeleteAsync(id);             
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
