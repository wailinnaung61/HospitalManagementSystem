using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Enums;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Infrastructure.Repositories;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Interfaces;
using System.Collections.Generic;

namespace MSIS_HMS.Controllers
{
    public class WarehouseTransfersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IWarehouseTransferRepository _warehouseTransferRepository;
        private readonly IBranchService _branchService;
        private readonly IWarehouseRepository _warehouseRepository;

        public WarehouseTransfersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IWarehouseTransferRepository warehouseTransferRepository, IBranchService branchService, IWarehouseRepository warehouseRepository)
        {
            _userManager = userManager;
            _context = context;
            _warehouseTransferRepository = warehouseTransferRepository;
            _branchService = branchService;
            _warehouseRepository = warehouseRepository;
        }

        public void Initialize(WarehouseTransfer warehouseTransfer = null)
        {
            ViewData["FromWarehouses"] = new SelectList(_warehouseRepository.GetAll(_branchService.GetBranchIdByUser()), "Id", "Name", warehouseTransfer?.FromWarehouseId);
            ViewData["ToWarehouses"] = new SelectList(_warehouseRepository.GetAll(_branchService.GetBranchIdByUser()), "Id", "Name", warehouseTransfer?.ToWarehouseId);
        }

        // GET
        public IActionResult Index()
        {
            var warehouseTransfers = _warehouseTransferRepository.GetAll(_branchService.GetBranchIdByUser()).Where(x => x.ToBranchId != null).ToList();
            //var warehouses = _warehouseRepository.GetAll(_branchService.GetBranchIdByUser());
            //warehouseTransfers.ForEach(x => { x.FromWarehouse = warehouses.SingleOrDefault(w => w.Id == x.FromWarehouseId); x.ToWarehouse = warehouses.SingleOrDefault(w => w.Id == x.ToWarehouseId); });
            return View(warehouseTransfers.OrderByDescending(x => x.Date).ToList());
        }

        public IActionResult Create()
        {
            Initialize();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WarehouseTransfer warehouseTransfer)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (warehouseTransfer.FromWarehouseId == warehouseTransfer.ToWarehouseId)
                        {
                            ModelState.AddModelError("ToWarehouseId", "Two different warehouses must be selected");
                        }
                        else if (warehouseTransfer.WarehouseTransferItems != null && warehouseTransfer.WarehouseTransferItems.Count() > 0)
                        {                            
                            warehouseTransfer = await _userManager.AddUserAndTimestamp(warehouseTransfer, User, DbEnum.DbActionEnum.Create);
                            var _warehouseTransfer = await _warehouseTransferRepository.AddAsync(warehouseTransfer);
                            if (_warehouseTransfer != null)
                            {
                                await _warehouseRepository.UpdateStockAsync(GetWarehouseItemForUpdate(warehouseTransfer, "from").Union(GetWarehouseItemForUpdate(warehouseTransfer, "to")).ToList());
                                await transaction.CommitAsync();
                                TempData["notice"] = StatusEnum.NoticeStatus.Success;
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        else
                        {
                            ViewData["Error"] = "WarehouseTransfer Items are required";
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    await transaction.RollbackAsync();
                }
            }
            Initialize(warehouseTransfer);
            return View(warehouseTransfer);
        }

        public IActionResult Edit(int id)
        {
            var warehouseTransfer = _warehouseTransferRepository.Get(id);
            Initialize(warehouseTransfer);
            return View(warehouseTransfer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(WarehouseTransfer warehouseTransfer)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (warehouseTransfer.FromWarehouseId == warehouseTransfer.ToWarehouseId)
                        {
                            ModelState.AddModelError("ToWarehouseId", "Two different warehouses must be selected");
                        }
                        else if (warehouseTransfer.WarehouseTransferItems != null && warehouseTransfer.WarehouseTransferItems.Count() > 0)
                        {
                            warehouseTransfer = await _userManager.AddUserAndTimestamp(warehouseTransfer, User, DbEnum.DbActionEnum.Update);

                            var _warehouseTransfer = await _warehouseTransferRepository.UpdateAsync(warehouseTransfer);
                            if (_warehouseTransfer != null)
                            {
                                await _warehouseRepository.UpdateStockAsync(GetWarehouseItemForUpdate(warehouseTransfer, "from").Union(GetWarehouseItemForUpdate(warehouseTransfer, "to")).ToList());
                                await transaction.CommitAsync();
                                TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {
                            ViewData["Error"] = "WarehouseTransfer Items are required";
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    await transaction.RollbackAsync();
                }
            }
            Initialize(warehouseTransfer);
            return View(warehouseTransfer);
        }

        public async Task<IActionResult> Delete(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var warehouseTransfer = _warehouseTransferRepository.Get(id);
                    var warehouseItemsForDelete = GetWarehouseItemForDelete(warehouseTransfer, "from").Union(GetWarehouseItemForDelete(warehouseTransfer, "to")).ToList();
                    var isSucceed = await _warehouseTransferRepository.DeleteAsync(id);
                    if (isSucceed)
                    {
                        await _warehouseRepository.UpdateStockAsync(warehouseItemsForDelete);
                        await transaction.CommitAsync();
                        TempData["notice"] = StatusEnum.NoticeStatus.Delete;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    await transaction.RollbackAsync();
                    TempData["notice"] = StatusEnum.NoticeStatus.Fail;
                }
                return RedirectToAction(nameof(Index));
            }
        }

        private List<WarehouseItem> GetWarehouseItemForUpdate(WarehouseTransfer warehouseTransfer, string type) // type = from / to
        {
            var warehouseId = type == "from" ? warehouseTransfer.FromWarehouseId : warehouseTransfer.ToWarehouseId;
            var _warehouseItemsForUpdate = new List<WarehouseItem>();
            var _existingWarehouseTransferItemsInStock = _warehouseTransferRepository.GetWarehouseTransferItemsForUpdate(warehouseTransfer.Id, warehouseId);
            var _newWarehouseTransferItemsForStock = warehouseTransfer.WarehouseTransferItems.GroupBy(x => new { warehouseId, x.ItemId, x.BatchId }, (key, g) => new WarehouseItem
            {
                WarehouseId = warehouseId,
                ItemId = g.First().ItemId,
                BatchId = g.First().BatchId,
                Qty = g.Sum(i => i.QtyInSmallestUnit)
            }).ToList();
            foreach (var warehouseTransferItem in _newWarehouseTransferItemsForStock)
            {
                var _existingWarehouseTransferItemInStock = _existingWarehouseTransferItemsInStock.SingleOrDefault(x => x.WarehouseId == warehouseId && x.ItemId == warehouseTransferItem.ItemId && x.BatchId == warehouseTransferItem.BatchId);
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
                        BatchId = warehouseTransferItem.BatchId,
                        Qty = (int)qtyChanged
                    });
                }
            }
            return _warehouseItemsForUpdate;
        }

        private List<WarehouseItem> GetWarehouseItemForDelete(WarehouseTransfer warehouseTransfer, string type)
        {
            warehouseTransfer.WarehouseTransferItems.ToList().ForEach(x => x.QtyInSmallestUnit = 0);
            return GetWarehouseItemForUpdate(warehouseTransfer, type);
        }
    }
}