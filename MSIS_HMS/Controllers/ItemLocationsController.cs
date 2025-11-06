using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Enums;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Repositories;
using MSIS_HMS.Models;
using NLog;
using X.PagedList;

namespace MSIS_HMS.Controllers
{
    public class ItemLocationsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly Pagination _pagination;
        private readonly IItemRepository _itemRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IItemLocationRepository _itemLocationRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly ILogger<ItemLocationsController> _logger;
        

        
        public ItemLocationsController(UserManager<ApplicationUser> userManager,ApplicationDbContext context,IOptions<Pagination> pagination,IItemRepository itemRepository,
                                      ILocationRepository locationRepository,IItemLocationRepository itemLocationRepository,IBranchRepository branchRepository,ILogger<ItemLocationsController> logger)
        {
            _userManager = userManager;
            _context = context;
            _pagination = pagination.Value;
            _itemRepository = itemRepository;
            _locationRepository = locationRepository;
            _itemLocationRepository = itemLocationRepository;
            _branchRepository = branchRepository;
            _logger = logger;
        }
        
        public void Initialize(ItemLocation itemLocation=null)
        {          
            var items = _itemRepository.GetAll();
            var locations = _locationRepository.GetAll();
            var batches = _context.Batches.Where(x => x.IsDelete == false).ToList();
            var warehouses = _context.Warehouses.Where(x => x.IsDelete == false).ToList();
            ViewData["items"] = new SelectList(items, "Id", "Name", itemLocation?.ItemId);
            ViewData["locations"] = new SelectList(locations, "Id", "Name", itemLocation?.LocationId);
            ViewData["batches"] = new SelectList(batches, "Id", "Name", itemLocation?.BatchId);
            ViewData["warehouses"] = new SelectList(warehouses, "Id", "Name", itemLocation?.WarehouseId);
        }

        public IActionResult Index(int? page = 1, int? ItemId = null, int? LocationId = null,int? BatchId=null,int? WarehouseId=null)
        {
            Initialize();
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            
            var itemLocations = _itemLocationRepository.GetAll(null,ItemId,LocationId,null,BatchId,WarehouseId);
            var branches = _branchRepository.GetAll();
            //var locations = _locationRepository.GetAll();
            //var items = _itemRepository.GetAll();

            itemLocations.ForEach(x =>
            {
                x.Branch = branches.SingleOrDefault(b => b.Id == x.BranchId);
                //x.Item = items.SingleOrDefault(i => i.Id == x.ItemId);
                //x.Location = locations.SingleOrDefault(l => l.Id == x.LocationId);
            });

            return View(itemLocations.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }

        public IActionResult Create()
        {
            Initialize();
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemLocation itemLocation)
        {
            MappedDiagnosticsLogicalContext.Set("userId", _userManager.GetUserId(User));//insert userId into Logs Table
            try
            {
                ModelState.Remove("BranchId");
                if (ModelState.IsValid)
                {
                    itemLocation = await _userManager.AddUserAndTimestamp(itemLocation, User, DbEnum.DbActionEnum.Create);
                    var _itemLocation = await _itemLocationRepository.AddAsync(itemLocation);
                    if (_itemLocation != null)
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
            Initialize();
            return View();
        }
        
        
        public IActionResult Edit(int id)
        {
            var itemLocation = _itemLocationRepository.Get(id);
            Initialize(itemLocation);
            return View(itemLocation);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ItemLocation itemLocation)
        {
            MappedDiagnosticsLogicalContext.Set("userId", _userManager.GetUserId(User));//insert userId into Logs Table
            try
            {
                if (ModelState.IsValid)
                {
                    itemLocation = await _userManager.AddUserAndTimestamp(itemLocation, User, DbEnum.DbActionEnum.Update);
                    await _itemLocationRepository.UpdateAsync(itemLocation);                   
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
            return View(itemLocation);
        }
        public async Task<ActionResult> Delete(int id)
        {
            MappedDiagnosticsLogicalContext.Set("userId", _userManager.GetUserId(User));//insert userId into Logs Table
            try
            {
                await _itemLocationRepository.DeleteAsync(id);             
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Delete));

            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }
            return RedirectToAction(nameof(Index));

        }
        
        public IActionResult GetBatch(int ItemId)
        {
            var batches = _context.Batches.Where(x => x.IsDelete == false && x.ItemId == ItemId).ToList();
            return Ok(batches);
        }
        public IActionResult GetLocation(int WarehouseId)
        {
            var locations = _context.Locations.Where(x => x.IsDelete == false && x.WarehouseId == WarehouseId).ToList();
            return Ok(locations);
        }
    }
}