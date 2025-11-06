using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Enums;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Infrastructure.Repositories;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Interfaces;
using MSIS_HMS.Models;
using X.PagedList;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using MSIS_HMS.Helpers;
using System.Collections.Generic;
using Microsoft.Reporting.NETCore;
using MSIS_HMS.Core.Entities.DTOs;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace MSIS_HMS.Controllers
{
    public class ItemsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IItemRepository _itemRepository;
        private readonly IItemTypeRepository _itemTypeRepository;
        private readonly IBranchService _branchService;
        private readonly IUserService _userService;
        private readonly IOutletRepository _outletRepository;
        private readonly Pagination _pagination;
        private readonly ILogger<ItemsController> _logger;
        private readonly IBatchRepository _batchRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ItemsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IItemRepository itemRepository, IItemTypeRepository itemTypeRepository, IBranchService branchService, IUserService userService, IOptions<Pagination> pagination, ILogger<ItemsController> logger, IBatchRepository batchRepository,IOutletRepository outletRepository,IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _context = context;
            _itemRepository = itemRepository;
            _itemTypeRepository = itemTypeRepository;
            _branchService = branchService;
            _userService = userService;
            _pagination = pagination.Value;
            _logger = logger;
            _batchRepository = batchRepository;
            _outletRepository = outletRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public void Initialize(Item item = null)
        {
            int? itemTypeId = null, branchId = null;
            if (item != null)
            {
                itemTypeId = item.ItemTypeId;
                branchId = item.BranchId;
            }
            ViewData["ItemTypes"] = _itemTypeRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", itemTypeId);
            ViewData["Branches"] = _branchService.GetSelectListItems(branchId);
        }

        // GET
        public IActionResult Index(int? page = 1, string ItemName = null, string BarCode = null, string Code = null, int? ItemTypeId = null, int? Price = null)
        {
            
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            var items = _itemRepository.GetAll(_userService.Get(User).BranchId, null, ItemTypeId, ItemName, Code, BarCode);
            var branches = _branchService.GetAll();
            ViewData["ItemTypes"] = _itemTypeRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", ItemTypeId);
            return View(items.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }
        public IActionResult DailySaleItemReport(int? page = 1, DateTime? FromDate=null,DateTime? ToDate=null,int? OutletId=null)
        {
            if(FromDate==null)
            {
                FromDate = DateTime.Now.Date;
                ToDate = DateTime.Now.Date;
            }
            var outlets = _outletRepository.GetAll(_branchService.GetBranchIdByUser());
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["Outlets"] = new SelectList(outlets, "Id", "Name");
            var items = _itemRepository.GetSaleItem(_userService.Get(User).BranchId, FromDate, ToDate, OutletId);
            TempData["Page"] = page;
            TempData["FromDate"] = FromDate;
            TempData["ToDate"] = ToDate;
            TempData["OutletId"] = OutletId;
            return View(items.ToList().ToPagedList((int)page, pageSize));
        }
        public IActionResult ExpirationRemindDay(int? page = 1, string ItemName = null, string BarCode = null, string Code = null, int? ItemTypeId = null, int? Price = null)
        {
            Initialize();
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            var items = _itemRepository.GetExpirationRemindDay(_userService.Get(User).BranchId, null, ItemTypeId, ItemName, Code, BarCode);
            var branches = _branchService.GetAll();
            items.ForEach(x => x.Branch = branches.SingleOrDefault(b => b.Id == x.BranchId));
            ViewData["ItemTypes"] = _itemTypeRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name", ItemTypeId);
            return View(items.ToPagedList((int)page, pageSize));

        }


        public IActionResult Create()
        {
            Initialize();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Item item)
        {

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var filename = item.ImageFile != null ? FtpHelper.ftpItemImageFolderPath + item.ImageFile.GetUniqueName() : "";
                try
                {
                    if (ModelState.IsValid)
                    {
                        var didUploaded = true;
                        if (item.ImageFile != null)
                        {
                            didUploaded = false;
                            var uploadRes = FtpHelper.UploadFileToServer(item.ImageFile, filename);
                            if (uploadRes.IsSucceed())
                            {
                                didUploaded = true;
                                item.Image = uploadRes.ResponseUri.AbsolutePath;
                            }
                        }
                        if (didUploaded)
                        {
                            item = await _userManager.AddUserAndTimestamp(item, User, DbEnum.DbActionEnum.Create);
                            await _itemRepository.AddAsync(item);
                            await transaction.CommitAsync();
                            TempData["notice"] = StatusEnum.NoticeStatus.Success;
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("Not successful save Blog");
                    Console.WriteLine(e.Message);
                    await transaction.RollbackAsync();
                    if (FtpHelper.CheckIfFileExistsOnServer(filename))
                    {
                        var deleteRes = FtpHelper.DeleteFileOnServer(filename);
                        if (deleteRes.IsSucceed())
                        {
                            _logger.LogError("Not successful delete item thumbnail");
                            Console.WriteLine(e.Message);
                        }
                    }
                    Initialize(item);
                    return View(item);
                }
            }
            Initialize(item);
            return View(item);
        }

        public IActionResult Edit(int id)
        {
            var item = _itemRepository.GetWithPackingUnit(id);
            item.ImageContent = item.Image.GetBase64();
            Initialize(item);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Item item)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var filename = item.ImageFile != null ? FtpHelper.ftpItemImageFolderPath + item.ImageFile.GetUniqueName() : "";
                try
                {
                    if (ModelState.IsValid)
                    {
                        var didUploaded = true;
                        if (item.ImageFile != null)
                        {
                            didUploaded = false;
                            var uploadRes = FtpHelper.UploadFileToServer(item.ImageFile, filename);
                            if (uploadRes.IsSucceed())
                            {
                                var didDeleted = true;
                                if (FtpHelper.CheckIfFileExistsOnServer(item.Image))
                                {
                                    didDeleted = false;
                                    var deleteRes = FtpHelper.DeleteFileOnServer(item.Image);
                                    if (deleteRes.IsSucceed())
                                    {
                                        didDeleted = true;
                                    }
                                }
                                if (didDeleted)
                                {
                                    didUploaded = true;
                                    item.Image = uploadRes.ResponseUri.AbsolutePath;
                                }
                            }
                        }
                        if (didUploaded)
                        {
                            item = await _userManager.AddUserAndTimestamp(item, User, DbEnum.DbActionEnum.Update);
                            await _itemRepository.UpdateAsync(item);
                            await transaction.CommitAsync();
                            _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                            return RedirectToAction("Index");
                        }
                        throw new Exception();
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("Not successful save Blog");
                    Console.WriteLine(e.Message);
                    await transaction.RollbackAsync();
                    if (FtpHelper.CheckIfFileExistsOnServer(filename))
                    {
                        var deleteRes = FtpHelper.DeleteFileOnServer(filename);
                        if (deleteRes.IsSucceed())
                        {
                            _logger.LogError("Not successful delete item thumbnail");
                            Console.WriteLine(e.Message);
                        }
                    }
                    Initialize(item);
                    return View(item);
                }
            }
            Initialize(item);
            return View(item);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var _item = await _itemRepository.DeleteAsync(id);
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
            var units = _itemRepository.GetAllWithPackingUnits(_userService.Get(User).BranchId);
            return Ok(units.OrderBy(x => x.Name).ToList());
        }

        public ActionResult Image(string path)
        {
            return File(FtpHelper.DownloadFileFromServer(path), "image/png");
        }

        public IActionResult DownloadReport()
        {
            var page = TempData["Page"];

            DateTime? fromDate = Convert.ToDateTime(TempData["FromDate"]);
            DateTime? toDate = Convert.ToDateTime(TempData["ToDate"]);
            int? outletId = null;
            if (TempData["FromDate"] == null)
            {
                fromDate = DateTime.Now.Date;
                toDate = DateTime.Now.Date;

            }
            if (TempData["OutletId"] != null)
            {
                outletId = (int)TempData["OutletId"];
            }
            string renderFormat = "PDF";
            string extension = "pdf";
            string mimetype = "application/pdf";
            using (var report = new LocalReport())
            {
                List<SaleItemDTO> Orders = new List<SaleItemDTO>();
                List<Branch> branches = new List<Branch>();
              
                var user = _userService.Get(User);
                Orders = _itemRepository.GetSaleItem(_userService.Get(User).BranchId, fromDate, toDate, outletId);
             
                var branch = _branchService.GetBranchById((int)user.BranchId);
                branches.Add(branch);
                ReportParameter[] parameters = new ReportParameter[2];
                parameters[0] = new ReportParameter("FromDate", fromDate.ToString());
                parameters[1] = new ReportParameter("ToDate", toDate.ToString());
               
                report.DataSources.Add(new ReportDataSource("Branch", branches));
                report.DataSources.Add(new ReportDataSource("dsSaleItem", Orders));

                report.ReportPath = $"{_webHostEnvironment.WebRootPath}\\ReportFiles\\SaleItemReport.rdlc";
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat); TempData["Page"] = page;
                TempData["FromDate"] = fromDate;
                TempData["ToDate"] = toDate;
                TempData["OutletId"] = outletId;
                return File(pdf, mimetype, "DailySaleItemReport_" + DateTime.Now + "." + extension);
            }
        }
        
    }
}