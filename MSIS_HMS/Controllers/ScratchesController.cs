using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
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

namespace MSIS_HMS.Controllers
{
    public class ScratchesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IBranchService _branchService;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IWarehouseService _warehouseService;
        private readonly IScratchRepository _scratchRepository;
        private readonly Pagination _pagination;

        public ScratchesController(UserManager<ApplicationUser> userManager,ApplicationDbContext context,IBranchService branchService,IWarehouseRepository warehouseRepository,IWarehouseService warehouseService,IOptions<Pagination> pagination,IScratchRepository scratchRepository)
        {
            _userManager = userManager;
            _context = context;
            _branchService = branchService;
            _warehouseRepository = warehouseRepository;
            _warehouseService = warehouseService;
            _pagination = pagination.Value;
            _scratchRepository = scratchRepository;
        }

        public void Initialize(Scratch scratch=null)
        {
            ViewData["Warehouses"] = new SelectList(_warehouseRepository.GetAll(_branchService.GetBranchIdByUser()), "Id", "Name", scratch?.WarehouseId);
        }
        public IActionResult Index(int? page = 1, DateTime? FromDate = null, DateTime? ToDate = null, int? warehouseId = null)
        {
            var purchases = _scratchRepository.GetAll(_branchService.GetBranchIdByUser(), null, warehouseId, null,  FromDate, ToDate);
            var warehouses = _warehouseRepository.GetAll(_branchService.GetBranchIdByUser());
            //purchases.ForEach(x => x.Warehouse = warehouses.SingleOrDefault(w => w.Id == x.WarehouseId));
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            //ViewData["Warehouse"] = _warehouseService.GetSelectListItems(warehouseId);
            ViewData["Warehouses"] = new SelectList(warehouses, "Id", "Name");
            return View(purchases.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }
        public IActionResult Create()
        {
            Initialize();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Scratch scratch)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (scratch.ScratchItems != null && scratch.ScratchItems.Count > 0)
                        {


                            scratch = await _userManager.AddUserAndTimestamp(scratch, User, DbEnum.DbActionEnum.Create);
                            var _scratch = await _scratchRepository.AddAsync(scratch);
                            if (_scratch != null)
                            {
                                await _warehouseRepository.UpdateStockAsync(scratch.ScratchItems.GetWarehouseItemForUpdate(_scratchRepository.GetScratchItemsForUpdate(_scratch.Id), _scratch.WarehouseId), (int)DbEnum.DbActionEnum.Delete);
                                await transaction.CommitAsync();
                                TempData["notice"] = StatusEnum.NoticeStatus.Success;
                                //_logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        else
                        {
                            ViewData["Error"] = "Scratch Items are required";
                        }
                    }
                }
                catch (Exception e)
                {
                   // _logger.LogError(e.InnerException.Message);
                    await transaction.RollbackAsync();
                }
            }
            Initialize();
            return View(scratch);
        }
        public IActionResult Edit(int id)
        {
            Initialize();
            var scratch = _scratchRepository.Get(id);
            return View(scratch);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Scratch scratch)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (scratch.ScratchItems != null && scratch.ScratchItems.Count() > 0)
                        {
                          
                            scratch = await _userManager.AddUserAndTimestamp(scratch, User, DbEnum.DbActionEnum.Update);

                            var _scratch = await _scratchRepository.UpdateAsync(scratch);
                            if (_scratch != null)
                            {
                                await _warehouseRepository.UpdateStockAsync(scratch.ScratchItems.GetWarehouseItemForUpdate(_scratchRepository.GetScratchItemsForUpdate(_scratch.Id), _scratch.WarehouseId), (int)DbEnum.DbActionEnum.Delete);
                                await transaction.CommitAsync();
                                TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                                //_logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {
                            ViewData["Error"] = "Scratch Items are required";
                        }
                    }
                }
                catch (Exception e)
                {
                    //_logger.LogError(e.InnerException.Message);
                    await transaction.RollbackAsync();
                }
            }
            Initialize();
            return View(scratch);
        }
        public async Task<IActionResult> Delete(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var scratch = _scratchRepository.Get(id);
                    var covertWareHouseItems = _scratchRepository.GetScratchItemsForUpdate(scratch.Id);
                    var warehouseItemsForDelete = scratch.ScratchItems.GetWarehouseItemForDelete(covertWareHouseItems, scratch.WarehouseId);
                    var isSucceed = await _scratchRepository.DeleteAsync(id);
                    if (isSucceed)
                    {
                        await _warehouseRepository.UpdateStockAsync(warehouseItemsForDelete, (int)DbEnum.DbActionEnum.Delete);
                        await transaction.CommitAsync();
                        TempData["notice"] = StatusEnum.NoticeStatus.Delete;
                        //_logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Delete));
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (Exception e)
                {
                    //_logger.LogError(e.InnerException.Message);
                    await transaction.RollbackAsync();
                    TempData["notice"] = StatusEnum.NoticeStatus.Fail;
                }
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
