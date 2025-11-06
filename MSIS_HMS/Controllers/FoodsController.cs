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
    public class FoodsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFoodRepository _foodRepository;

        private readonly ApplicationDbContext _context;
        private readonly Pagination _pagination;
        private readonly IItemService _itemService;
        private readonly IUserService _userService;
        private readonly IFoodCategoryRepository _foodCategoryRepository;
        private readonly ILogger<FoodsController> _logger;
        public FoodsController(UserManager<ApplicationUser> userManager, IFoodRepository foodRepository, ApplicationDbContext context, IOptions<Pagination> pagination, IItemService itemService, IUserService userService, ILogger<FoodsController> logger, IFoodCategoryRepository foodCategoryRepository)
        {
            _userManager = userManager;
            _foodRepository = foodRepository;
            _context = context;
            _pagination = pagination.Value;
            _itemService = itemService;
            _userService = userService;
            _logger = logger;
            _foodCategoryRepository = foodCategoryRepository;
        }

        public void Initialize()
        {
            var foodcategory = _context.FoodCategories.Where(x => x.IsDelete == false).ToList();
            ViewData["FoodCategories"] = new SelectList(foodcategory, "Id", "Name");
        }

        public IActionResult Index(int? FoodCategoryId=null,string Name=null,decimal? UnitPrice=null,string Code=null,string Description=null, int? page = 1)
        {
            Initialize();
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            var food = _foodRepository.GetAll(null, FoodCategoryId, Name, UnitPrice, Code, Description).ToList();
            return View(food.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }
        public IActionResult Create()
        {
            Initialize();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Food food)
        {

            if (ModelState.IsValid)
            {
                food = await _userManager.AddUserAndTimestamp(food, User, DbEnum.DbActionEnum.Create);
                var _food = await _foodRepository.AddAsync(food);
                if (food != null)
                {
                    TempData["notice"] = StatusEnum.NoticeStatus.Success;

                }
                return RedirectToAction(nameof(Index));
            }
            return View(food);
        }

        public IActionResult Edit(int id)
        {
            Initialize();
            var food = _foodRepository.Get(id);
            return View(food);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Food food)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    food = await _userManager.AddUserAndTimestamp(food, User, DbEnum.DbActionEnum.Update);
                    var _food = await _foodRepository.UpdateAsync(food);
                    TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }

            return View(food);
        }
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var _food = await _foodRepository.DeleteAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Delete));
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult GetAll()
        {
            var doctors = _foodRepository.GetAll(_userService.Get(User).BranchId);
            return Ok(doctors);
        }
    }

}