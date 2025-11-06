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
    public class ReturnsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IBranchService _branchService;
        private readonly IOutletRepository _outletRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IBatchRepository _batchRepository;
        private readonly IReturnRepository _returnRepository;
        private readonly ILogger<ReturnsController> _logger;
        private readonly Pagination _pagination;
        

        public ReturnsController(UserManager<ApplicationUser> userManager,ApplicationDbContext context,IBranchService branchService,IOutletRepository outletRepository,IWarehouseRepository warehouseRepository,IBatchRepository batchRepository,IReturnRepository returnRepository,IOptions<Pagination> pagination,ILogger<ReturnsController> logger)
        {
            _userManager = userManager;
            _context = context;
            _branchService = branchService;
            _outletRepository = outletRepository;
            _warehouseRepository = warehouseRepository;
            _batchRepository = batchRepository;
            _returnRepository = returnRepository;
            _pagination = pagination.Value;
            _logger = logger;
        }
        public void Initialize(Return returnModel = null)
        {
            ViewData["ToWarehouses"] = new SelectList(_warehouseRepository.GetAll(_branchService.GetBranchIdByUser()), "Id", "Name", returnModel?.WarehouseId);
            ViewData["FromOutlets"] = new SelectList(_outletRepository.GetAll(_branchService.GetBranchIdByUser()), "Id", "Name", returnModel?.OutletId);
        }

        public IActionResult Index(int? page = 1, int? warehouseId = null, int? outletId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var outletTransfers = _returnRepository.GetAll(_branchService.GetBranchIdByUser(), warehouseId, outletId, fromDate, toDate);
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

        public async Task<IActionResult> Create(Return returnModel)
        {
            MappedDiagnosticsLogicalContext.Set("userId", _userManager.GetUserId(User));
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {

                        if (returnModel.ReturnItems != null && returnModel.ReturnItems.Count() > 0)
                        {
                            returnModel = await _userManager.AddUserAndTimestamp(returnModel, User, DbEnum.DbActionEnum.Create);
                            var _warehouseTransfer = await _returnRepository.AddAsync(returnModel);
                            if (_warehouseTransfer != null)
                            {
                                var warehouseItem = GetWarehouseItemForUpdate(returnModel, "to").ToList();
                                var outletItem = GetOutletItemForUpdate(returnModel, "from").ToList();
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
                            ViewData["Error"] = "Return Items are required";
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.InnerException.Message);
                    await transaction.RollbackAsync();
                }
            }
            Initialize(returnModel);
            return View(returnModel);
        }

        public IActionResult Edit(int id)
        {
            var outletTransfer = _returnRepository.Get(id);
            Initialize(outletTransfer);
            return View(outletTransfer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Return returnModel)
        {
            MappedDiagnosticsLogicalContext.Set("userId", _userManager.GetUserId(User));
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {

                        if (returnModel.ReturnItems != null && returnModel.ReturnItems.Count() > 0)
                        {
                            returnModel = await _userManager.AddUserAndTimestamp(returnModel, User, DbEnum.DbActionEnum.Update);

                            var _warehouseTransfer = await _returnRepository.UpdateAsync(returnModel);
                            if (_warehouseTransfer != null)
                            {
                                var warehouseItem = GetWarehouseItemForUpdate(returnModel, "to").ToList();
                                var outletItem = GetOutletItemForUpdate(returnModel, "from").ToList();
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
            Initialize(returnModel);
            return View(returnModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            MappedDiagnosticsLogicalContext.Set("userId", _userManager.GetUserId(User));
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var outletTransfer = _returnRepository.Get(id);

                    var outletItemsForDelete = GetOutletItemForDelete(outletTransfer, "from").ToList();
                    var warehoueItemsForDetete = GetWarehouseItemForDelete(outletTransfer, "to").ToList();
                    var isSucceed = await _returnRepository.DeleteAsync(id);
                    if (isSucceed)
                    {
                        //await _outletRepository.UpdateStockAsync(outletItemsForDelete, warehoueItemsForDetete);
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
        private List<WarehouseItem> GetWarehouseItemForUpdate(Return returnModel, string type) // type = from / to
        {
            var warehouseId = returnModel.WarehouseId ;
            var _warehouseItemsForUpdate = new List<WarehouseItem>();
            var _existingWarehouseTransferItemsInStock = _returnRepository.GetWarehouseTransferItemsForUpdate(returnModel.Id, returnModel.WarehouseId);
            var _newWarehouseTransferItemsForStock = returnModel.ReturnItems.GroupBy(x => new { warehouseId, x.ItemId, x.BatchId }, (key, g) => new WarehouseItem
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
               
                if (qtyChanged != null)
                {
                    _warehouseItemsForUpdate.Add(new WarehouseItem
                    {
                        WarehouseId = warehouseId,
                        ItemId = warehouseTransferItem.ItemId,
                        BatchId = warehouseTransferItem.BatchId,
                        Qty = (int)qtyChanged
                    });
                }

            }
            return _warehouseItemsForUpdate;
        }
        private List<OutletItem> GetOutletItemForUpdate(Return returnModel, string type) // type = from / to
        {
            var outletId = returnModel.OutletId;
            var _outletItemsForUpdate = new List<OutletItem>();
            var _existingWarehouseTransferItemsInStock = _returnRepository.GetOutletTransferItemsForUpdate(returnModel.Id, outletId);
            var _newOutletTransferItemsForStock = returnModel.ReturnItems.GroupBy(x => new { outletId, x.ItemId, x.BatchId }, (key, g) => new OutletItem
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
                if(type=="from")
                {
                    qtyChanged = -qtyChanged;
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

        private List<WarehouseItem> GetWarehouseItemForDelete(Return returnModel, string type) // type = from / to
        {
            returnModel.ReturnItems.ToList().ForEach(x => x.QtyInSmallestUnit = 0);
            return GetWarehouseItemForUpdate(returnModel, type);
        }
        private List<OutletItem> GetOutletItemForDelete(Return returnModel, string type) // type = from / to
        {
            returnModel.ReturnItems.ToList().ForEach(x => x.QtyInSmallestUnit = 0);
            return GetOutletItemForUpdate(returnModel, type);
        }
    }
}
