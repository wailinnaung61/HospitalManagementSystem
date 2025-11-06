﻿using Microsoft.AspNetCore.Identity;
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
using System.Linq;
using System.Threading.Tasks;
 using Microsoft.AspNetCore.Mvc.Rendering;
 using Microsoft.Extensions.Options;
 using X.PagedList;

namespace MSIS_HMS.Controllers
{
    public class WardsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWardRepository _wardRepository;
        private readonly IFloorRepository _floorRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ApplicationDbContext _context;
        private readonly Pagination _pagination;
        private readonly IItemService _itemService;
        private readonly IUserService _userService;
        private readonly ILogger<WardsController> _logger;
        public WardsController(UserManager<ApplicationUser> userManager, IWardRepository wardRepository,ApplicationDbContext context, IOptions<Pagination> pagination, IItemService itemService, IUserService userService, ILogger<WardsController> logger,IDepartmentRepository departmentRepository,IFloorRepository floorRepository)
        {
            _userManager = userManager;
            _wardRepository = wardRepository;
            _context = context;
            _pagination = pagination.Value;
            _itemService = itemService;
            _userService = userService;
            _logger = logger;
            _departmentRepository = departmentRepository;
            _floorRepository = floorRepository;
        }
        
        public void Initialize(Ward ward = null)
        {
            var departments = _context.Departments.Where(x => x.IsDelete == false).ToList();
            ViewData["Departments"] = new SelectList(departments, "Id", "Name", ward?.DepartmentId);
            var floors = _context.Floors.Where(x => x.IsDelete == false).ToList();
            ViewData["Floors"] = new SelectList(floors, "Id", "Name", ward?.FloorId);
            var outlets = _context.Outlets.Where(x => x.IsDelete == false).ToList();
            ViewData["Outlets"] = new SelectList(outlets, "Id", "Name", ward?.OutletId);
        }
        
        public IActionResult Index(string WardName = null,int? DepartmentId=null,int? FloorId=null, int? page = 1)
        {
            Initialize();
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            
            var ward = _wardRepository.GetAll( null, WardName,DepartmentId,FloorId).ToList();
            //var floors = _floorRepository.GetAll();
            //var departments = _departmentRepository.GetAll(_userService.Get(User).BranchId);
            return View(ward.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }
        
        public IActionResult Create()
        {
            Initialize();
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ward ward)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        ward = await _userManager.AddUserAndTimestamp(ward, User, DbEnum.DbActionEnum.Create);
                        var _ward = await _wardRepository.AddAsync(ward);
                        await transaction.CommitAsync();
                        if (ward != null)
                        {
                            TempData["notice"] = StatusEnum.NoticeStatus.Success;
                        }
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(e.InnerException.Message);
                }
            }
            return View(ward);
        }
        
        public IActionResult Edit(int id)
        {
            var ward = _wardRepository.Get(id);
            Initialize(ward);
            return View(ward);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Ward ward)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        ward = await _userManager.AddUserAndTimestamp(ward, User, DbEnum.DbActionEnum.Update);
                        var _ward = await _wardRepository.UpdateAsync(ward);
                        await transaction.CommitAsync();
                        TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(e.InnerException.Message);
                }
            }
            Initialize(ward);
            return View(ward);
        }
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var _ward = await _wardRepository.DeleteAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }

            return RedirectToAction(nameof(Index));
        }
    }

}