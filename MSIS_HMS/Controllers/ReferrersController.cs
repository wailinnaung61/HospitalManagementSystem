using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Enums;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using MSIS_HMS.Models;
using Microsoft.Extensions.Options;
using X.PagedList;
using MSIS_HMS.Core.Entities.DTOs;
using Microsoft.Reporting.NETCore;

namespace MSIS_HMS.Controllers
{
    public class ReferrersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly IReferrerRepository _referrerRepository;
        private readonly ILogger<ReferrersController> _logger;
        private readonly IConfiguration Configuration;
        private IHostingEnvironment _hostingEnv;
        private readonly Pagination _pagination;
        private readonly IBranchService _branchService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReferrersController(IBranchService branchService, IOptions<Pagination> pagination,UserManager<ApplicationUser> userManager, IUserService userService,IReferrerRepository referrerRepository,ILogger<ReferrersController> logger, IConfiguration _configuration, IHostingEnvironment hostingEnvironment, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _userService = userService;
            _referrerRepository = referrerRepository;
            _logger = logger;
            _branchService = branchService;
            Configuration = _configuration;
            _hostingEnv = hostingEnvironment;
            _pagination = pagination.Value;
            _webHostEnvironment = webHostEnvironment;
        }

        public void Initialize(Referrer refferrer=null)
        {
        }

        public IActionResult Index()
        {
            var referrers = _referrerRepository.GetAll();
            return View(referrers);
        }
        public IActionResult Create()
        {
            Initialize();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Referrer referrer)
        {
            MappedDiagnosticsLogicalContext.Set("userId", _userManager.GetUserId(User));
            try
            {
                if (ModelState.IsValid)
                {
                    referrer = await _userManager.AddUserAndTimestamp(referrer, User, DbEnum.DbActionEnum.Create);
                    var _referrer = await _referrerRepository.AddAsync(referrer);
                    if (_referrer != null)
                    {
                        TempData["notice"] = StatusEnum.NoticeStatus.Success;
                        _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _logger.LogError(e.InnerException.Message);
            }
            Initialize();
            return View();
        }
        public IActionResult Edit(int id)
        {
            Initialize();
            var referrer = _referrerRepository.Get(id);
            return View(referrer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Referrer referrer)
        {
            MappedDiagnosticsLogicalContext.Set("userId", _userManager.GetUserId(User));
            try
            {
                if (ModelState.IsValid)
                {
                    referrer = await _userManager.AddUserAndTimestamp(referrer, User, DbEnum.DbActionEnum.Update);
                    var _staff = await _referrerRepository.UpdateAsync(referrer);
                    TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                    _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _logger.LogError(e.InnerException.Message);
            }
            Initialize();
            return View(referrer);
        }

        public async Task<ActionResult> Delete(int id)
        {
            MappedDiagnosticsLogicalContext.Set("userId", _userManager.GetUserId(User));
            try
            {
                var _staff = await _referrerRepository.DeleteAsync(id);
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
            var referrers = _referrerRepository.GetAll(_userService.Get(User).BranchId);
            return Ok(referrers.OrderBy(x => x.Name).ToList());
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
                Referrer referrer = new Referrer();
                referrer = await _userManager.AddUserAndTimestamp(referrer, User, DbEnum.DbActionEnum.Create);
                dt.Columns.Add("BranchId", typeof(Int32));
                dt.Columns.Add("IsDelete", typeof(bool));
                dt.Columns.Add("CreatedAt", typeof(DateTime));
                dt.Columns.Add("CreatedBy", typeof(string));
                dt.Columns.Add("UpdatedAt", typeof(DateTime));
                dt.Columns.Add("UpdatedBy", typeof(string));
                foreach (var row in dt.AsEnumerable().ToList())
                {
                    row["BranchId"] = referrer.BranchId;
                    row["IsDelete"] = referrer.IsDelete;
                    row["CreatedAt"] = referrer.CreatedAt;
                    row["CreatedBy"] = referrer.CreatedBy;
                    row["UpdatedAt"] = referrer.UpdatedAt;
                    row["UpdatedBy"] = referrer.UpdatedBy;
                }
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                {
                    //Set the database table name.
                    sqlBulkCopy.DestinationTableName = "dbo.Referrer";

                    // Map the Excel columns with that of the database table, this is optional but good if you do
                    // 
                    sqlBulkCopy.ColumnMappings.Add("CreatedAt", "CreatedAt");
                    sqlBulkCopy.ColumnMappings.Add("CreatedBy", "CreatedBy");
                    sqlBulkCopy.ColumnMappings.Add("UpdatedAt", "UpdatedAt");
                    sqlBulkCopy.ColumnMappings.Add("UpdatedBy", "UpdatedBy");
                    sqlBulkCopy.ColumnMappings.Add("IsDelete", "IsDelete");
                    sqlBulkCopy.ColumnMappings.Add("Name", "Name");
                    sqlBulkCopy.ColumnMappings.Add("Address", "Address");
                    sqlBulkCopy.ColumnMappings.Add("Phone", "Phone");
                    sqlBulkCopy.ColumnMappings.Add("Township", "Township");
                    sqlBulkCopy.ColumnMappings.Add("City", "City");
                    sqlBulkCopy.ColumnMappings.Add("BranchId", "BranchId");
                    sqlBulkCopy.ColumnMappings.Add("Fee", "Fee");
                    sqlBulkCopy.ColumnMappings.Add("FeeType", "FeeType");

                    con.Open();
                    sqlBulkCopy.WriteToServer(dt);
                    con.Close();
                }
            }
                //if the code reach here means everthing goes fine and excel data is imported into database


            }

            return RedirectToAction("Index");

        }
        public IActionResult ReferrerReport(DateTime? FromDate = null, DateTime? ToDate = null,int? ReferrerId=null, int? page = 1)
        {
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;     
            ViewData["Referrers"] = _referrerRepository.GetAll(_userService.Get(User).BranchId).GetSelectListItems("Id", "Name");

            //data bind
            var referrers = _referrerRepository.GetReferrerReport(ReferrerId,FromDate,ToDate);
            
            TempData["StartDate"] = FromDate;
            TempData["EndDate"] = ToDate;
            TempData["Referrer"] = ReferrerId;
            return View(referrers.ToList().ToPagedList((int)page, pageSize));
        }

        public IActionResult DownloadReferrerReport()
        {
            var page = TempData["Page"];
            DateTime? fromDate = null;
            DateTime? toDate = null;
            if (TempData["StartDate"] != null)
            {
               fromDate = Convert.ToDateTime(TempData["StartDate"]);                
            }          

            if(TempData["EndDate"] != null)
            {
               toDate = Convert.ToDateTime(TempData["EndDate"]);
            }

            int? ReferrerId = null;

            if (TempData["Referrer"] != null)
            {
                ReferrerId = (int)TempData["Referrer"];
            }

            string renderFormat = "PDF";
            string extension = "pdf";
            string mimetype = "application/pdf";
            using (var report = new LocalReport())
            {
                List<ReferrerReportDTO> referrers = new List<ReferrerReportDTO>();
                List<Branch> branches = new List<Branch>();

                var user = _userService.Get(User);

                //data bind
                referrers = _referrerRepository.GetReferrerReport(ReferrerId, fromDate, toDate);

                var branch = _branchService.GetBranchById((int)_userService.Get(User).BranchId);
                branches.Add(branch);
                ReportParameter[] parameters = new ReportParameter[2];
                parameters[0] = new ReportParameter("FromDate", fromDate.ToString());
                parameters[1] = new ReportParameter("ToDate", toDate.ToString());

                report.DataSources.Add(new ReportDataSource("Branch", branches));
                report.DataSources.Add(new ReportDataSource("ReferrerReport", referrers));             
                report.ReportPath = $"{_webHostEnvironment.WebRootPath}\\ReportFiles\\ReferrerReport.rdlc";
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                TempData["Page"] = page;
                TempData["StartDate"] = fromDate;
                TempData["EndDate"] = toDate;
                TempData["Referrer"] = ReferrerId;
                return File(pdf, mimetype, "ReferrerReport_" + DateTime.Now + "." + extension);
            }
        }

    }
}
