using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Enums;
using MSIS_HMS.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Repositories;
using MSIS_HMS.Interfaces;
using MSIS_HMS.Models;
using MSIS_HMS.Services;
using X.PagedList;
using static MSIS_HMS.Infrastructure.Enums.DbEnum;

namespace MSIS_HMS.Controllers
{
    public class DeliverOrdersController : Controller
    {

        private readonly IWarehouseRepository _warehouseRepository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DeliverOrdersController> _logger;
        private readonly IDeliverOrderRepository _deliverOrderRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBranchService _branchService;
        private readonly IWarehouseService _warehouseService;
        private readonly Pagination _pagination;

        public DeliverOrdersController(IWarehouseRepository warehouseRepository, ApplicationDbContext context, ILogger<DeliverOrdersController> logger, IDeliverOrderRepository deliverOrderRepository, UserManager<ApplicationUser> userManager, IBranchService branchService, IWarehouseService warehouseService, IOptions<Pagination> pagination)
        {
            _warehouseRepository = warehouseRepository;
            _context = context;
            _logger = logger;
            _deliverOrderRepository = deliverOrderRepository;
            _userManager = userManager;
            _branchService = branchService;
            _warehouseService = warehouseService;
            _pagination = pagination.Value;
        }



        public void Initialize(DeliverOrder deliverOrder = null)
        {
            var warehouses = _warehouseRepository.GetAll(_branchService.GetBranchIdByUser());
            ViewData["Warehouses"] = new SelectList(warehouses, "Id", "Name", deliverOrder?.WarehouseId);
            var branch = _branchService.GetBranchByUser();
            ViewData["UseVoucherFormat"] = branch.UseVoucherFormatForPurchase;
        }

        // GET
        public IActionResult Index(int? page = 1, DateTime? FromDate = null, DateTime? ToDate = null, string VoucherNo = null, int? warehouseId = null)
        {
            var purchases = _deliverOrderRepository.GetAll(_branchService.GetBranchIdByUser(), null, warehouseId, null, VoucherNo, null, null, FromDate, ToDate);
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
            var deliverOrder = new DeliverOrder
            {
                VoucherNo = _branchService.GetVoucherNo(VoucherTypeEnum.Deliver)
            };
            return View(deliverOrder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeliverOrder deliverOrder)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (deliverOrder.DeliverOrderItems != null && deliverOrder.DeliverOrderItems.Count > 0)
                        {
                            deliverOrder.VoucherNo = _branchService.GetVoucherNo(VoucherTypeEnum.Deliver, deliverOrder.VoucherNo);
                            if (string.IsNullOrEmpty(deliverOrder.VoucherNo))
                            {
                                ModelState.AddModelError("VoucherNo", "This field is required.");
                                Initialize(deliverOrder);
                                return View(deliverOrder);
                            }
                            deliverOrder.Total = InventoryExtensions.CalculateBulkTotal(deliverOrder);
                            deliverOrder = await _userManager.AddUserAndTimestamp(deliverOrder, User, DbEnum.DbActionEnum.Create);
                            var _deliverOrder = await _deliverOrderRepository.AddAsync(deliverOrder);
                            if (_deliverOrder != null)
                            {
                                await _warehouseRepository.UpdateStockAsync(deliverOrder.DeliverOrderItems.GetWarehouseItemForUpdate(_deliverOrderRepository.GetDeliverOrderItemsForUpdate(_deliverOrder.Id), _deliverOrder.WarehouseId), (int)DbEnum.DbActionEnum.Delete);
                                await _branchService.IncreaseVoucherNo(VoucherTypeEnum.Deliver);
                                await transaction.CommitAsync();
                                TempData["notice"] = StatusEnum.NoticeStatus.Success;
                                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        else
                        {
                            ViewData["Error"] = "DeliverOrder Items are required";
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.InnerException.Message);
                    await transaction.RollbackAsync();
                }
            }
            Initialize();
            return View(deliverOrder);
        }
        public IActionResult Edit(int id)
        {
            Initialize();
            var deliverOrder = _deliverOrderRepository.Get(id);
            return View(deliverOrder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DeliverOrder deliverOrder)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (deliverOrder.DeliverOrderItems != null && deliverOrder.DeliverOrderItems.Count() > 0)
                        {
                            deliverOrder.Total = InventoryExtensions.CalculateBulkTotal(deliverOrder);
                            deliverOrder = await _userManager.AddUserAndTimestamp(deliverOrder, User, DbEnum.DbActionEnum.Update);

                            var _deliverOrder = await _deliverOrderRepository.UpdateAsync(deliverOrder);
                            if (_deliverOrder != null)
                            {
                                await _warehouseRepository.UpdateStockAsync(deliverOrder.DeliverOrderItems.GetWarehouseItemForUpdate(_deliverOrderRepository.GetDeliverOrderItemsForUpdate(_deliverOrder.Id), _deliverOrder.WarehouseId), (int)DbEnum.DbActionEnum.Delete);
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
            Initialize();
            return View(deliverOrder);
        }

        public async Task<IActionResult> Delete(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var deliverOrder = _deliverOrderRepository.Get(id);
                    var covertWareHouseItems = _deliverOrderRepository.GetDeliverOrderItemsForUpdate(deliverOrder.Id);
                    var warehouseItemsForDelete = deliverOrder.DeliverOrderItems.GetWarehouseItemForDelete(covertWareHouseItems, deliverOrder.WarehouseId);
                    var isSucceed = await _deliverOrderRepository.DeleteAsync(id);
                    if (isSucceed)
                    {
                        await _warehouseRepository.UpdateStockAsync(warehouseItemsForDelete, (int)DbEnum.DbActionEnum.Delete);
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
    }
}