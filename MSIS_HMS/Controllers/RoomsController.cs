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
 using DocumentFormat.OpenXml.Wordprocessing;
 using Microsoft.AspNetCore.Mvc.Rendering;
 using Microsoft.Extensions.Options;
 using MSIS_HMS.Core.Enums;
 using X.PagedList;
 using EnumExtension = MSIS_HMS.Infrastructure.Enums.EnumExtension;

 namespace MSIS_HMS.Controllers
{
    public class RoomsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRoomRepository _roomRepository;

        private readonly ApplicationDbContext _context;
        private readonly Pagination _pagination;
        private readonly IItemService _itemService;
        private readonly IUserService _userService;
        private readonly IWardRepository _wardRepository;
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly ILogger<RoomsController> _logger;
        public RoomsController(UserManager<ApplicationUser> userManager, IRoomRepository roomRepository, ApplicationDbContext context, IOptions<Pagination> pagination, IItemService itemService, IUserService userService, ILogger<RoomsController> logger,IWardRepository wardRepository,IRoomTypeRepository roomTypeRepository)
        {
            _userManager = userManager;
            _roomRepository = roomRepository;
            _context = context;
            _pagination = pagination.Value;
            _itemService = itemService;
            _userService = userService;
            _logger = logger;
            _wardRepository = wardRepository;
            _roomTypeRepository = roomTypeRepository;
        }
        
        public void Initialize()
        {
            var roomtypes = _context.RoomTypes.Where(x => x.IsDelete == false).ToList();
            ViewData["RoomTypes"] = new SelectList(roomtypes, "Id", "Name");
            var wards = _context.Wards.Where(x => x.IsDelete == false).ToList();
            ViewData["Wards"] = new SelectList(wards, "Id", "Name");
            var enumData = from RoomStatusEnum e in Enum.GetValues(typeof(RoomStatusEnum))
                select new
                {
                    ID = (int)e,
                    Name = EnumExtension.ToDescription(e),
                };
            ViewData["RoomStatus"] = new SelectList(enumData, "Name", "Name");  
        }
        
        public IActionResult Index(int? RoomTypeId = null, string Status = null, decimal? Price = null,int? WardId=null, int? page = 1,string RoomStatus = null,string RoomNo = null)
        
        {
            Initialize();
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            var room = _roomRepository.GetAll(null,RoomTypeId,RoomStatus,Price, WardId,RoomNo).ToList();
            //var wards = _wardRepository.GetAll();
            //var roomtypes = _roomTypeRepository.GetAll();
            //room.ForEach(x => x.Ward = wards.SingleOrDefault(b => b.Id == x.WardId));
            //room.ForEach(x=>x.RoomType=roomtypes.SingleOrDefault(b=>b.Id==x.RoomTypeId));
            return View(room.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }
        public IActionResult Create()
        {
            Initialize();
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Room room)
        {
           
                if (ModelState.IsValid)
                {
                    room = await _userManager.AddUserAndTimestamp(room, User, DbEnum.DbActionEnum.Create);
                    var _room = await _roomRepository.AddAsync(room);
                    if (room != null)
                    {
                        TempData["notice"] = StatusEnum.NoticeStatus.Success;

                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(room);
        }
        
        public IActionResult Edit(int id)
        {
            Initialize();
            var room = _roomRepository.Get(id);
            return View(room);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Room room)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    room = await _userManager.AddUserAndTimestamp(room, User, DbEnum.DbActionEnum.Update);
                    var _room = await _roomRepository.UpdateAsync(room);
                    TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }

            return View(room);
        }
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var _room = await _roomRepository.DeleteAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Delete));
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }

            return RedirectToAction(nameof(Index));
        }
    }

}