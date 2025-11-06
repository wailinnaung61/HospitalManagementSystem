using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Enums;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Interfaces;
using MSIS_HMS.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace MSIS_HMS.Controllers
{
    public class GroundingsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IBranchService _branchService;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IWarehouseTransferRepository _warehouseTransferRepository;
        private readonly IGroundingRepository _groundingRepository;
        private readonly IOutletTransferRepository _outletTransferRepository;
        private readonly Pagination _pagination;
        public readonly ILogger<GroundingsController> _logger;
        public GroundingsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IBranchService branchService, IWarehouseRepository warehouseRepository, IWarehouseTransferRepository warehouseTransferRepository, IOptions<Pagination> pagination, IGroundingRepository groundingRepository, IOutletTransferRepository outletTransferRepository,ILogger<GroundingsController> logger)
        {
            _userManager = userManager;
            _context = context;
            _branchService = branchService;
            _warehouseRepository = warehouseRepository;
            _warehouseTransferRepository = warehouseTransferRepository;
            _pagination = pagination.Value;
            _groundingRepository = groundingRepository;
            _outletTransferRepository = outletTransferRepository;
            _logger = logger;
        }
        public void Initialize(Grounding grounding = null)
        {
            ViewData["FromWarehouses"] = new SelectList(_warehouseRepository.GetAll(_branchService.GetBranchIdByUser()), "Id", "Name", grounding?.WarehouseId);

        }


        public IActionResult Create()
        {
            Initialize();
            return View();
        }
        public IActionResult Index(int? page = 1, int? warehouseId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var groundings = _groundingRepository.GetAll(_branchService.GetBranchIdByUser(), warehouseId , fromDate, toDate);
            var warehouses = _warehouseRepository.GetAll(_branchService.GetBranchIdByUser());          
            ViewData["Warehouses"] = new SelectList(warehouses, "Id", "Name");        
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            groundings.ForEach(x => x.GroundingItems = _context.GroundingItems.Include(x=>x.Item).Where(a => a.GroundingId == x.Id).ToList());
            return View(groundings.OrderByDescending(x => x.Date).ToPagedList((int)page, pageSize));
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Grounding grounding)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {

                        if (grounding.GroundingItems != null && grounding.GroundingItems.Count() > 0)
                        {
                            grounding = await _userManager.AddUserAndTimestamp(grounding, User, DbEnum.DbActionEnum.Create);
                            var _warehouseTransfer = await _groundingRepository.AddAsync(grounding);
                            if (_warehouseTransfer != null)
                            {
                                var warehouseItem = GetWarehouseItemForUpdate(grounding, "from").ToList();
                                await _warehouseRepository.ReplaceStockAsync(warehouseItem);

                                await transaction.CommitAsync();
                                TempData["notice"] = StatusEnum.NoticeStatus.Success;
                                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        else
                        {
                            ViewData["Error"] = "Grounding Items are required";
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    _logger.LogError(e.InnerException.Message);
                    await transaction.RollbackAsync();
                }
            }
            Initialize(grounding);
            return View(grounding);
        }

      
        private List<WarehouseItem> GetWarehouseItemForUpdate(Grounding grounding, string type) // type = from / to
        {
            var warehouseId = type == "from" ? grounding.WarehouseId : 0;
            var _warehouseItemsForUpdate = new List<WarehouseItem>();
            var _newWarehouseTransferItemsForStock = grounding.GroundingItems.GroupBy(x => new { warehouseId, x.ItemId, x.BatchId }, (key, g) => new WarehouseItem
            {
                WarehouseId = warehouseId,
                ItemId = g.First().ItemId,
                BatchId = g.First().BatchId,
                Qty = g.Sum(i => i.ChangedQty)
            }).ToList();
            foreach (var warehouseTransferItem in _newWarehouseTransferItemsForStock)
            {
                // var _existingWarehouseTransferItemInStock = _existingWarehouseTransferItemsInStock.SingleOrDefault(x => x.WarehouseId == warehouseId && x.ItemId == warehouseTransferItem.ItemId);

                _warehouseItemsForUpdate.Add(new WarehouseItem
                {
                    WarehouseId = warehouseId,
                    ItemId = warehouseTransferItem.ItemId,
                    BatchId = warehouseTransferItem.BatchId,
                    Qty = (int)warehouseTransferItem.Qty
                });


            }
            return _warehouseItemsForUpdate;
        }
       
    }
}
