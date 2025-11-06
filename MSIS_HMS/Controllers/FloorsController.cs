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
 using Microsoft.Extensions.Options;
 using X.PagedList;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Hosting;
using NPOI.SS.UserModel;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace MSIS_HMS.Controllers
{
    public class FloorsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFloorRepository _floorRepository;
        private readonly IBranchService _branchService;
        private readonly ApplicationDbContext _context;
        private readonly Pagination _pagination;
        private readonly IItemService _itemService;
        private readonly IUserService _userService;
        private readonly ILogger<FloorsController> _logger;
        private readonly IConfiguration Configuration;
        private IHostingEnvironment _hostingEnv;
        
        public FloorsController(UserManager<ApplicationUser> userManager,IFloorRepository floorRepository,IBranchService branchService,ApplicationDbContext context,IOptions<Pagination> pagination,IItemService itemService,IUserService userService,ILogger<FloorsController> logger, IConfiguration _configuration, IHostingEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _floorRepository = floorRepository;
            _branchService = branchService;
            _context = context;
            _pagination = pagination.Value;
            _itemService = itemService;
            _userService = userService;
            _logger = logger;
            Configuration = _configuration;
            _hostingEnv = hostingEnvironment;
        }
        public IActionResult Index(string FloorName=null,int? page=1)
        {
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            var floor = _floorRepository.GetAll(_userService.Get(User).BranchId,null,FloorName).ToList();
            var branches = _branchService.GetAll();
            floor.ForEach(x => x.Branch = branches.SingleOrDefault(b => b.Id == x.BranchId));
            return View(floor.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
           
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Floor floor)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var branch = _branchService.GetBranchByUser();
                        floor = await _userManager.AddUserAndTimestamp(floor, User, DbEnum.DbActionEnum.Create);
                        var _floor = await _floorRepository.AddAsync(floor);
                        await transaction.CommitAsync();
                        if(floor != null)
                        {
                            TempData["notice"] = StatusEnum.NoticeStatus.Success;
                        }
                        return RedirectToAction("Index", "Floors");
                    }
                }
                catch (Exception e)
                {
                     await transaction.RollbackAsync();
                    _logger.LogError(e.InnerException.Message);
                }
            }
            return View(floor);
        }
        public IActionResult Edit(int id)
        {
            var floor = _floorRepository.Get(id);
            return View(floor);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Floor floor)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        floor = await _userManager.AddUserAndTimestamp(floor, User, DbEnum.DbActionEnum.Update);
                        var _floor = await _floorRepository.UpdateAsync(floor);
                        await transaction.CommitAsync();                        
                        TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                        return RedirectToAction("Index","Floors");
                    }
                }
                catch (Exception e)
                {
                     await transaction.RollbackAsync();
                    _logger.LogError(e.InnerException.Message);
                }
            }
            return View(floor);
        }
        
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var _floor = await _floorRepository.DeleteAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
            }
            catch (Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        public ActionResult Upload()
        {
            IFormFile file = Request.Form.Files[0];
            string folderName = "UploadExcel";
            string webRootPath = _hostingEnv.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            StringBuilder sb = new StringBuilder();
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            if (file.Length > 0)
            {
                string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                ISheet sheet;
                string fullPath = Path.Combine(newPath, file.FileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    stream.Position = 0;
                    if (sFileExtension == ".xls")
                    {
                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  
                    }
                    else
                    {
                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   
                    }
                    IRow headerRow = sheet.GetRow(0); //Get Header Row
                    int cellCount = headerRow.LastCellNum;
                    sb.Append("<table class='table table-bordered'><tr>");
                    for (int j = 0; j < cellCount; j++)
                    {
                        NPOI.SS.UserModel.ICell cell = headerRow.GetCell(j);
                        if (cell == null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                        sb.Append("<th>" + cell.ToString() + "</th>");
                    }
                    sb.Append("</tr>");
                    sb.AppendLine("<tr>");
                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++) //Read Excel File
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            if (row.GetCell(j) != null)
                                sb.Append("<td>" + row.GetCell(j).ToString() + "</td>");
                        }
                        sb.AppendLine("</tr>");
                    }
                    sb.Append("</table>");
                }
            }
            return this.Content(sb.ToString());
        }
        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImportExcelFile(IFormFile FormFile)
        {
            if(FormFile == null)
            {
                return RedirectToAction("Import");
            }
            else
            {

            
            //get file name
            var filename = ContentDispositionHeaderValue.Parse(FormFile.ContentDisposition).FileName.Trim('"');
            //get path
            var MainPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");

            //create directory "Uploads" if it doesn't exists
            if (!Directory.Exists(MainPath))
            {
                Directory.CreateDirectory(MainPath);
            }

            //get file path 
            var filePath = Path.Combine(MainPath, FormFile.FileName);
            using (System.IO.Stream stream = new FileStream(filePath, FileMode.Create))
            {
                await FormFile.CopyToAsync(stream);
            }

            //get extension
            string extension = Path.GetExtension(filename);


            string conString = string.Empty;

            switch (extension)
            {
                case ".xls": //Excel 97-03.
                    conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES'";
                    break;
                case ".xlsx": //Excel 07 and above.
                    conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES'";
                    break;
            }

            DataTable dt = new DataTable();
            conString = string.Format(conString, filePath);

            using (OleDbConnection connExcel = new OleDbConnection(conString))
            {
                using (OleDbCommand cmdExcel = new OleDbCommand())
                {
                    using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                    {
                        cmdExcel.Connection = connExcel;

                        //Get the name of First Sheet.
                        connExcel.Open();
                        DataTable dtExcelSchema;
                        dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                        connExcel.Close();

                        //Read Data from First Sheet.
                        connExcel.Open();
                        cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                        odaExcel.SelectCommand = cmdExcel;
                        odaExcel.Fill(dt);
                        connExcel.Close();
                    }
                }
            }
            //your database connection string
            conString = this.Configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection con = new SqlConnection(conString))
            {
                Floor floor = new Floor();
                floor = await _userManager.AddUserAndTimestamp(floor, User, DbEnum.DbActionEnum.Create);
                dt.Columns.Add("BranchId", typeof(Int32));
                dt.Columns.Add("IsDelete", typeof(bool));
                dt.Columns.Add("CreatedAt", typeof(DateTime));
                dt.Columns.Add("CreatedBy", typeof(string));
                dt.Columns.Add("UpdatedAt", typeof(DateTime));
                dt.Columns.Add("UpdatedBy", typeof(string));
                foreach (var row in dt.AsEnumerable().ToList())
                {
                    row["BranchId"] = floor.BranchId;
                    row["IsDelete"] = floor.IsDelete;
                    row["CreatedAt"] = floor.CreatedAt;
                    row["CreatedBy"] = floor.CreatedBy;
                    row["UpdatedAt"] = floor.UpdatedAt;
                    row["UpdatedBy"] = floor.UpdatedBy;
                }
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                {
                    //Set the database table name.
                    sqlBulkCopy.DestinationTableName = "dbo.Floor";

                    // Map the Excel columns with that of the database table, this is optional but good if you do
                    // 
                    sqlBulkCopy.ColumnMappings.Add("CreatedAt", "CreatedAt");
                    sqlBulkCopy.ColumnMappings.Add("CreatedBy", "CreatedBy");
                    sqlBulkCopy.ColumnMappings.Add("UpdatedAt", "UpdatedAt");
                    sqlBulkCopy.ColumnMappings.Add("UpdatedBy", "UpdatedBy");
                    sqlBulkCopy.ColumnMappings.Add("IsDelete", "IsDelete");
                    sqlBulkCopy.ColumnMappings.Add("BranchId", "BranchId");
                    sqlBulkCopy.ColumnMappings.Add("Name", "Name");
                    con.Open();
                    sqlBulkCopy.WriteToServer(dt);
                    con.Close();
                }
            }
                //if the code reach here means everthing goes fine and excel data is imported into database
            }


            return RedirectToAction("Index");

        }

    }
}