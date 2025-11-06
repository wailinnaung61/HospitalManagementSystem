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
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using MSIS_HMS.Models;
using X.PagedList;
using Microsoft.Extensions.Logging;

namespace MSIS_HMS.Controllers
{
    [Authorize]
    public class BatchesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IItemService _itemService;
        private readonly IBatchRepository _batchRepository;
        private readonly IBranchService _branchService;
        private readonly ILogger<BatchesController> _logger;
        private readonly Pagination _pagination;

        public BatchesController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IItemService itemService, IBatchRepository batchRepository, IBranchService branchService, IOptions<Pagination> pagination,ILogger<BatchesController> logger)
        {
            _userManager = userManager;
            _context = context;
            _itemService = itemService;
            _batchRepository = batchRepository;
            _branchService = branchService;
            _pagination = pagination.Value;
            _logger = logger;
        }

        public void Initialize(Batch batch = null)
        {
            ViewData["Items"] = _itemService.GetSelectListItems(batch?.ItemId);
        }

        // GET
        public IActionResult Index(int? page = 1, int? ItemId=null, string BatchName = null, string BatchCode = null, string BatchNumber = null, DateTime? StartExpiryDate = null, DateTime? EndExpiryDate = null)
        {
            var batches = _batchRepository.GetAll(_branchService.GetBranchIdByUser(), ItemId, null, BatchName, BatchCode, BatchNumber, null, StartExpiryDate, EndExpiryDate);
            //var items = _itemService.GetAll();
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["Items"] = _itemService.GetSelectListItems(ItemId);
            return View(batches.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }

        public IActionResult Create()
        {
            Initialize();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Batch batch)
        {
            try
            {
                ModelState.Remove("BranchId");
                if (ModelState.IsValid)
                {
                    batch = await _userManager.AddUserAndTimestamp(batch, User, DbEnum.DbActionEnum.Create);
                    var _batch = await _batchRepository.AddAsync(batch);
                    if (_batch != null)
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
            Initialize(batch);
            return View(batch);
        }

        public IActionResult Edit(int id)
        {
            var batch = _batchRepository.Get(id);
            Initialize(batch);
            return View(batch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Batch batch)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    batch = await _userManager.AddUserAndTimestamp(batch, User, DbEnum.DbActionEnum.Update);
                    var _batch = await _batchRepository.UpdateAsync(batch);
                    TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                    _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }
            Initialize(batch);
            return View(batch);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var _batch = await _batchRepository.DeleteAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Delete));
                
            }
            catch(Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult GetAll(int? ItemId)
        {
            var batches = _batchRepository.GetAll(_branchService.GetBranchIdByUser(), ItemId);
            return Ok(batches.OrderByDescending(x => x.UpdatedAt).ToList());
        }

       
    }
}