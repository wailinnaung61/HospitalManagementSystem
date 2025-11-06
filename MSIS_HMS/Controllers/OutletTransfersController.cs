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
    public class OutletTransfersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IOutletTransferRepository _outletTransferRepository;
        private readonly IBranchService _branchService;
        private readonly IOutletRepository _outletRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IWarehouseTransferRepository _warehouseTransferRepository;
        private readonly Pagination _pagination;
        private readonly ILogger<OutletTransfersController> _logger;

        public OutletTransfersController(UserManager<ApplicationUser> userManager,ApplicationDbContext context,IOutletTransferRepository outletTransferRepository,IBranchService branchService,IOutletRepository outletRepository,IWarehouseRepository warehouseRepository,IWarehouseTransferRepository warehouseTransferRepository, IOptions<Pagination> pagination,ILogger<OutletTransfersController> logger)
        {
            _userManager = userManager;
            _context = context;
            _outletTransferRepository = outletTransferRepository;
            _branchService = branchService;
            _outletRepository = outletRepository;
            _warehouseRepository = warehouseRepository;
            _warehouseTransferRepository = warehouseTransferRepository;
            _pagination = pagination.Value;
            _logger = logger;
        }

        public void Initialize(OutletTransfer outletTransfer = null)
        {
            ViewData["FromWarehouses"] = new SelectList(_warehouseRepository.GetAll(_branchService.GetBranchIdByUser()), "Id", "Name", outletTransfer?.WarehouseId);
            ViewData["ToOutlets"] = new SelectList(_outletRepository.GetAll(_branchService.GetBranchIdByUser()), "Id", "Name", outletTransfer?.OutletId);
        }
        public IActionResult Index(int? page=1,int? warehouseId=null,int? outletId=null,DateTime? fromDate=null,DateTime? toDate=null)
        {
            var outletTransfers = _outletTransferRepository.GetAll(_branchService.GetBranchIdByUser(),warehouseId,outletId,fromDate,toDate);
            var warehouses = _warehouseRepository.GetAll(_branchService.GetBranchIdByUser());
            var outlets = _outletRepository.GetAll(_branchService.GetBranchIdByUser());
            ViewData["Warehouses"] = new SelectList(warehouses, "Id", "Name");
            ViewData["Outlets"] = new SelectList(outlets, "Id", "Name");
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            //outletTransfers.ForEach(x => { x.Warehouse = warehouses.SingleOrDefault(w => w.Id == x.WarehouseId); x.Outlet = outlets.SingleOrDefault(w => w.Id == x.OutletId); });
            return View(outletTransfers.OrderByDescending(x => x.Date).ToPagedList((int)page, pageSize));

        }
        public IActionResult Create()
        {
            Initialize();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(OutletTransfer outletTransfer)
        {
            MappedDiagnosticsLogicalContext.Set("userId", _userManager.GetUserId(User));
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                       
                        if (outletTransfer.OutletTransferItems != null && outletTransfer.OutletTransferItems.Count() > 0)
                        {
                            outletTransfer = await _userManager.AddUserAndTimestamp(outletTransfer, User, DbEnum.DbActionEnum.Create);
                            var _warehouseTransfer = await _outletTransferRepository.AddAsync(outletTransfer);
                            if (_warehouseTransfer != null)
                            {
                                var warehouseItem= GetWarehouseItemForUpdate(outletTransfer, "from").ToList();
                                var outletItem = GetOutletItemForUpdate(outletTransfer, "to"). ToList();
                                await _warehouseRepository.UpdateStockAsync(warehouseItem);
                                await _outletRepository.UpdateStockAsync(outletItem);
                                await transaction.CommitAsync();
                                TempData["notice"] = StatusEnum.NoticeStatus.Success;
                                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        else
                        {
                            ViewData["Error"] = "OutletTransfer Items are required";
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
            Initialize(outletTransfer);
            return View(outletTransfer);
        }

        public IActionResult Edit(int id)
        {
            var outletTransfer = _outletTransferRepository.Get(id);
            Initialize(outletTransfer);
            return View(outletTransfer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OutletTransfer outletTransfer)
        {
            MappedDiagnosticsLogicalContext.Set("userId", _userManager.GetUserId(User));
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                      
                        if (outletTransfer.OutletTransferItems != null && outletTransfer.OutletTransferItems.Count() > 0)
                        {
                            outletTransfer = await _userManager.AddUserAndTimestamp(outletTransfer, User, DbEnum.DbActionEnum.Update);

                            var _warehouseTransfer = await _outletTransferRepository.UpdateAsync(outletTransfer);
                            if (_warehouseTransfer != null)
                            {
                                var warehouseItem = GetWarehouseItemForUpdate(outletTransfer, "from").ToList();
                                var outletItem = GetOutletItemForUpdate(outletTransfer, "to").ToList();
                                await _warehouseRepository.UpdateStockAsync(warehouseItem);
                                await _outletRepository.UpdateStockAsync(outletItem);
                                await transaction.CommitAsync();
                                TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {
                            ViewData["Error"] = "OutletTransfer Items are required";
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
            Initialize(outletTransfer);
            return View(outletTransfer);
        }

        public async Task<IActionResult> Delete(int id)
        {
            MappedDiagnosticsLogicalContext.Set("userId", _userManager.GetUserId(User));
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var outletTransfer = _outletTransferRepository.Get(id);

                    var outletItemsForDelete = GetOutletItemForDelete(outletTransfer, "to").ToList();
                    var warehoueItemsForDetete = GetWarehouseItemForDelete(outletTransfer, "from").ToList();
                    var isSucceed = await _outletTransferRepository.DeleteAsync(id);
                    if (isSucceed)
                    {
                        //await _outletRepository.UpdateStockAsync(outletItemsForDelete,warehoueItemsForDetete);
                        await _warehouseRepository.UpdateStockAsync(warehoueItemsForDetete);
                        await _outletRepository.UpdateStockAsync(outletItemsForDelete);
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
                    Console.WriteLine(e.Message);
                    _logger.LogError(e.InnerException.Message);
                    await transaction.RollbackAsync();
                    TempData["notice"] = StatusEnum.NoticeStatus.Fail;
                }
                return RedirectToAction(nameof(Index));
            }
        }
        private List<WarehouseItem> GetWarehouseItemForUpdate(OutletTransfer outletTransfer, string type) // type = from / to
        {
            var warehouseId = type == "from" ? outletTransfer.WarehouseId : outletTransfer.OutletId;
            var _warehouseItemsForUpdate = new List<WarehouseItem>();
            var _existingWarehouseTransferItemsInStock = _outletTransferRepository.GetWarehouseTransferItemsForUpdate(outletTransfer.Id, outletTransfer.WarehouseId);
            var _newWarehouseTransferItemsForStock = outletTransfer.OutletTransferItems.GroupBy(x => new { warehouseId, x.ItemId, x.BatchId }, (key, g) => new WarehouseItem
            {
                WarehouseId = warehouseId,
                ItemId = g.First().ItemId,
                BatchId = g.First().BatchId,
                Qty = g.Sum(i => i.QtyInSmallestUnit)
            }).ToList();
            foreach (var warehouseTransferItem in _newWarehouseTransferItemsForStock)
            {
                var _existingWarehouseTransferItemInStock = _existingWarehouseTransferItemsInStock.SingleOrDefault(x => x.WarehouseId == warehouseId && x.ItemId == warehouseTransferItem.ItemId);
                int? qtyChanged = null;
                if (_existingWarehouseTransferItemInStock != null)
                {
                    // Update
                    qtyChanged = warehouseTransferItem.Qty - _existingWarehouseTransferItemInStock.Qty;
                }
                else
                {
                    // Add
                    qtyChanged = warehouseTransferItem.Qty;
                }
                if (type == "from")
                {
                    qtyChanged = -qtyChanged;
                }
                if (qtyChanged != null)
                {
                    _warehouseItemsForUpdate.Add(new WarehouseItem
                    {
                        WarehouseId = warehouseId,
                        ItemId = warehouseTransferItem.ItemId,
                        BatchId= warehouseTransferItem.BatchId,
                        Qty = (int)qtyChanged
                    });
                }
                
            }
            return _warehouseItemsForUpdate;
        }
        private List<OutletItem> GetOutletItemForUpdate(OutletTransfer outletTransfer, string type) // type = from / to
        {
            var outletId = type == "from" ? outletTransfer.WarehouseId : outletTransfer.OutletId;
            var _outletItemsForUpdate = new List<OutletItem>();
            var _existingWarehouseTransferItemsInStock = _outletTransferRepository.GetOutletTransferItemsForUpdate(outletTransfer.Id, outletId);
            var _newOutletTransferItemsForStock = outletTransfer.OutletTransferItems.GroupBy(x => new { outletId, x.ItemId, x.BatchId }, (key, g) => new OutletItem
            {
                OutletId = outletId,
                ItemId = g.First().ItemId,
                Qty = g.Sum(i => i.QtyInSmallestUnit)
            }).ToList();
            foreach (var outletTransferItem in _newOutletTransferItemsForStock)
            {
                var _existingWarehouseTransferItemInStock = _existingWarehouseTransferItemsInStock.SingleOrDefault(x => x.OutletId == outletId && x.ItemId == outletTransferItem.ItemId);
                int? qtyChanged = null;
                if (_existingWarehouseTransferItemInStock != null)
                {
                    // Update
                    qtyChanged = outletTransferItem.Qty - _existingWarehouseTransferItemInStock.Qty;
                }
                else
                {
                    // Add
                    qtyChanged = outletTransferItem.Qty;
                }           
                if (qtyChanged != null)
                {
                    _outletItemsForUpdate.Add(new OutletItem
                    {
                        OutletId = outletId,
                        ItemId = outletTransferItem.ItemId,
                        Qty = (int)qtyChanged
                    });
                }
            }
            return _outletItemsForUpdate;
        }
        private List<WarehouseItem> GetWarehouseItemForDelete(OutletTransfer outletTransfer, string type) // type = from / to
        {
            outletTransfer.OutletTransferItems.ToList().ForEach(x => x.QtyInSmallestUnit = 0);
            return GetWarehouseItemForUpdate(outletTransfer, type);
        }
        private List<OutletItem> GetOutletItemForDelete(OutletTransfer outletTransfer, string type) // type = from / to
        {
            outletTransfer.OutletTransferItems.ToList().ForEach(x => x.QtyInSmallestUnit = 0);
            return GetOutletItemForUpdate(outletTransfer, type);
        }

       

    }
}
