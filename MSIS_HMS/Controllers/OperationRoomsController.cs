using Microsoft.AspNetCore.Identity;
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
    public class OperationRoomsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOperationRoomRepository _operationRoomRepository;

        private readonly ApplicationDbContext _context;
        private readonly Pagination _pagination;
        private readonly IItemService _itemService;
        private readonly IUserService _userService;
        private readonly IWardRepository _wardRepository;
        private readonly ILogger<RoomsController> _logger;
        public OperationRoomsController(UserManager<ApplicationUser> userManager, IOperationRoomRepository operationRoomRepository, ApplicationDbContext context, IOptions<Pagination> pagination, IItemService itemService, IUserService userService, ILogger<RoomsController> logger, IWardRepository wardRepository)
        {
            _userManager = userManager;
            _operationRoomRepository = operationRoomRepository;
            _context = context;
            _pagination = pagination.Value;
            _itemService = itemService;
            _userService = userService;
            _logger = logger;
            _wardRepository = wardRepository;
            
        }

        public void Initialize()
        {
           
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

        public IActionResult Index( decimal? Price = null, int? WardId = null, int? page = 1, string RoomStatus = null, string RoomNo = null)

        {
            Initialize();
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            var operationRoom = _operationRoomRepository.GetAll(null, RoomStatus, Price, WardId, RoomNo).ToList();
            //var wards = _wardRepository.GetAll();
            //var roomtypes = _roomTypeRepository.GetAll();
            //room.ForEach(x => x.Ward = wards.SingleOrDefault(b => b.Id == x.WardId));
            //room.ForEach(x=>x.RoomType=roomtypes.SingleOrDefault(b=>b.Id==x.RoomTypeId));
            return View(operationRoom.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }
        public IActionResult Create()
        {
            Initialize();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OperationRoom operationRoom)
        {

            if (ModelState.IsValid)
            {
                operationRoom = await _userManager.AddUserAndTimestamp(operationRoom, User, DbEnum.DbActionEnum.Create);
                var _operationRoom = await _operationRoomRepository.AddAsync(operationRoom);
                if (operationRoom != null)
                {
                    TempData["notice"] = StatusEnum.NoticeStatus.Success;

                }
                return RedirectToAction(nameof(Index));
            }
            return View(operationRoom);
        }

        public IActionResult Edit(int id)
        {
            Initialize();
            var operationRoom = _operationRoomRepository.Get(id);
            return View(operationRoom);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OperationRoom operationRoom)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    operationRoom = await _userManager.AddUserAndTimestamp(operationRoom, User, DbEnum.DbActionEnum.Update);
                    var _operationRoom = await _operationRoomRepository.UpdateAsync(operationRoom);
                    TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }

            return View(operationRoom);
        }
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var _operationRoom = await _operationRoomRepository.DeleteAsync(id);
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