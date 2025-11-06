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
using Microsoft.Extensions.Logging;
using MSIS_HMS.Models;
using Microsoft.Extensions.Options;
using X.PagedList;

namespace MSIS_HMS.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IBranchService _branchService;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IWarehouseService _warehouseService;
        private readonly ILogger<PurchasesController> _logger;
        private readonly Pagination _pagination;

        public PurchasesController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IPurchaseRepository purchaseRepository, IBranchService branchService, IWarehouseRepository warehouseRepository,ILogger<PurchasesController> logger, IOptions<Pagination> pagination,IWarehouseService warehouseService)
        {
            _userManager = userManager;
            _context = context;
            _purchaseRepository = purchaseRepository;
            _branchService = branchService;
            _warehouseRepository = warehouseRepository;
            _logger = logger;
            _pagination = pagination.Value;
            _warehouseService = warehouseService;
        }

        public void Initialize(Purchase purchase = null)
        {
            ViewData["Warehouses"] = new SelectList(_warehouseRepository.GetAll(_branchService.GetBranchIdByUser()), "Id", "Name", purchase?.WarehouseId);
            var branch = _branchService.GetBranchByUser();
            ViewData["UseVoucherFormat"] = branch.UseVoucherFormatForPurchase;
        }

        // GET
        public IActionResult Index(int? page = 1, string FromDate = null, string ToDate = null, string VoucherNo = null,int? warehouseId=null)
        {
            var purchases = _purchaseRepository.GetAll(_branchService.GetBranchIdByUser(),null,warehouseId,null,VoucherNo,null,null,null,null);
            //var warehouses = _warehouseRepository.GetAll(_branchService.GetBranchIdByUser());
            //purchases.ForEach(x => x.Warehouse = warehouses.SingleOrDefault(w => w.Id == x.WarehouseId));
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["Warehouse"] = _warehouseService.GetSelectListItems(warehouseId);
            return View(purchases.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }

        public IActionResult Create()
        {
            Initialize();
            var purchase = new Purchase
            {
                VoucherNo = _branchService.GetVoucherNo(VoucherTypeEnum.Purchase)
            };
            return View(purchase);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Purchase purchase)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (purchase.PurchaseItems != null && purchase.PurchaseItems.Count() > 0)
                        {
                            purchase.VoucherNo = _branchService.GetVoucherNo(VoucherTypeEnum.Purchase, purchase.VoucherNo);
                            if (string.IsNullOrEmpty(purchase.VoucherNo))
                            {
                                ModelState.AddModelError("VoucherNo", "This field is required.");
                                Initialize(purchase);
                                return View(purchase);
                            }
                            purchase.Total = purchase.PurchaseItems.CalculateTotal(); //_purchaseRepository.CalculateTotal(purchase.PurchaseItems.ToList());
                            purchase = await _userManager.AddUserAndTimestamp(purchase, User, DbEnum.DbActionEnum.Create);
                            var _purchase = await _purchaseRepository.AddAsync(purchase);
                            if (_purchase != null)
                            {
                                await _warehouseRepository.UpdateStockAsync(purchase.PurchaseItems.GetWarehouseItemForUpdate(_purchaseRepository.GetPurchaseItemsForUpdate(_purchase.Id), _purchase.WarehouseId));
                                await _branchService.IncreaseVoucherNo(VoucherTypeEnum.Purchase);
                                await transaction.CommitAsync();
                                TempData["notice"] = StatusEnum.NoticeStatus.Success;
                                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        else
                        {
                            ViewData["Error"] = "Purchase Items are required";
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.InnerException.Message);
                    await transaction.RollbackAsync();
                }
            }
            Initialize(purchase);
            return View(purchase);
        }

        public IActionResult Edit(int id)
        {
            var purchase = _purchaseRepository.Get(id);
            Initialize(purchase);
            return View(purchase);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Purchase purchase)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (purchase.PurchaseItems != null && purchase.PurchaseItems.Count() > 0)
                        {
                            purchase.Total = purchase.PurchaseItems.CalculateTotal();
                            purchase = await _userManager.AddUserAndTimestamp(purchase, User, DbEnum.DbActionEnum.Update);

                            var _purchase = await _purchaseRepository.UpdateAsync(purchase);
                            if (_purchase != null)
                            {
                                await _warehouseRepository.UpdateStockAsync(purchase.PurchaseItems.GetWarehouseItemForUpdate(_purchaseRepository.GetPurchaseItemsForUpdate(purchase.Id), purchase.WarehouseId));
                                await transaction.CommitAsync();
                                TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {
                            ViewData["Error"] = "Purchase Items are required";
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.InnerException.Message);
                    await transaction.RollbackAsync();
                }
            }
            Initialize(purchase);
            return View(purchase);
        }

        public async Task<IActionResult> Delete(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var purchase = _purchaseRepository.Get(id);
                    var warehouseItemsForDelete = purchase.PurchaseItems.GetWarehouseItemForDelete(_purchaseRepository.GetPurchaseItemsForUpdate(purchase.Id), purchase.WarehouseId);
                    var isSucceed = await _purchaseRepository.DeleteAsync(id);
                    if(isSucceed)
                    {
                        await _warehouseRepository.UpdateStockAsync(warehouseItemsForDelete);
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

        //private List<WarehouseItem> GetWarehouseItemForUpdate(Purchase purchase)
        //{
        //    var _warehouseItemsForUpdate = new List<WarehouseItem>();
        //    var _existingPurchaseItemsInStock = _purchaseRepository.GetPurchaseItemsForUpdate(purchase.Id);
        //    var _newPurchaseItemsForStock = purchase.PurchaseItems.GroupBy(x => new { purchase.WarehouseId, x.ItemId, x.BatchId }, (key, g) => new WarehouseItem
        //    {
        //        WarehouseId = purchase.WarehouseId,
        //        ItemId = g.First().ItemId,
        //        BatchId = g.First().BatchId,
        //        Qty = g.Sum(i => i.QtyInSmallestUnit)
        //    }).ToList();
        //    foreach (var purchaseItem in _newPurchaseItemsForStock)
        //    {
        //        var _existingPurchaseItemInStock = _existingPurchaseItemsInStock.SingleOrDefault(x => x.WarehouseId == purchase.WarehouseId && x.ItemId == purchaseItem.ItemId && x.BatchId == purchaseItem.BatchId);
        //        int? qtyChanged = null;
        //        if (_existingPurchaseItemInStock != null)
        //        {
        //            // Update
        //            qtyChanged = purchaseItem.Qty - _existingPurchaseItemInStock.Qty;
        //        }
        //        else
        //        {
        //            // Add
        //            qtyChanged = purchaseItem.Qty;
        //        }
        //        if (qtyChanged != null)
        //        {
        //            _warehouseItemsForUpdate.Add(new WarehouseItem
        //            {
        //                WarehouseId = purchase.WarehouseId,
        //                ItemId = purchaseItem.ItemId,
        //                BatchId = purchaseItem.BatchId,
        //                Qty = (int)qtyChanged
        //            });
        //        }
        //    }
        //    return _warehouseItemsForUpdate;
        //}
    }
}