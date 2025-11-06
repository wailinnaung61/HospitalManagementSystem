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
    public class BedsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBedRepository _bedRepository;        
        private readonly ApplicationDbContext _context;
        private readonly IBedTypeRepository _bedTypeRepository;
        private readonly Pagination _pagination;
        private readonly IUserService _userService;
        private readonly IRoomRepository _roomRepository;
        private readonly ILogger<RoomsController> _logger;
        public BedsController(UserManager<ApplicationUser> userManager, IBedRepository bedRepository, ApplicationDbContext context, IOptions<Pagination> pagination, IItemService itemService, IUserService userService, ILogger<RoomsController> logger,IRoomRepository roomRepository,IBedTypeRepository bedTypeRepository)
        {
            _userManager = userManager;
            _bedRepository = bedRepository;
            _context = context;
            _pagination = pagination.Value;
            _userService = userService;
            _logger = logger;
            _roomRepository = roomRepository;
            _bedTypeRepository = bedTypeRepository;
        }

        public void Initialize()
        {
            var rooms = _context.Rooms.Where(x => x.IsDelete == false).ToList();
            ViewData["Rooms"] = new SelectList(rooms, "Id", "RoomNo");
            var bedtypes = _context.BedTypes.Where(x=>x.IsDelete == false).ToList();
            ViewData["BedTypes"] = new SelectList(bedtypes, "Id", "Name");
            var enumData = from RoomStatusEnum e in Enum.GetValues(typeof(RoomStatusEnum))
                           select new
                           {
                               ID = (int)e,
                               Name = EnumExtension.ToDescription(e),
                           };
            ViewData["BedStatus"] = new SelectList(enumData, "Name", "Name");
        }

        public IActionResult Index(int? BedId = null, int? BedTypeId = null, string BedStatus = null, int? RoomId = null, string BedNo = null,int? page = 1)
        {
            Initialize();
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            var bed = _bedRepository.GetAll(null,BedTypeId,BedStatus,RoomId,BedNo).ToList();
            //var rooms = _roomRepository.GetAll();
            //var bedtypes = _bedTypeRepository.GetAll();
            //bed.ForEach(x => x.Room = rooms.SingleOrDefault(b => b.Id == x.RoomId));
            //bed.ForEach(x => x.BedType = bedtypes.SingleOrDefault(b => b.Id == x.BedTypeId));

            return View(bed.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }
        public IActionResult Create()
        {
            Initialize();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bed bed)
        {

            if (ModelState.IsValid)
            {
                bed = await _userManager.AddUserAndTimestamp(bed, User, DbEnum.DbActionEnum.Create);
                var _bed = await _bedRepository.AddAsync(bed);
                if (_bed != null)
                {
                    TempData["notice"] = StatusEnum.NoticeStatus.Success;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(bed);
        }

        public IActionResult Edit(int id)
        {
            Initialize();
            var bed = _bedRepository.Get(id);
            return View(bed);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Bed bed)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bed = await _userManager.AddUserAndTimestamp(bed, User, DbEnum.DbActionEnum.Update);
                    var _bed = await _bedRepository.UpdateAsync(bed);
                    TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }

            return View(bed);
        }
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var _bed = await _bedRepository.DeleteAsync(id);
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