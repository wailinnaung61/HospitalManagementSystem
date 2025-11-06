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
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace MSIS_HMS.Controllers
{
    public class DiagnosticsController : Controller
    {
        private readonly IDiagnosticRepository _diagnosticRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Pagination _pagination;
        private readonly IUserService _userService;
        private readonly IConfiguration Configuration;
        private IHostingEnvironment _hostingEnv;

        public DiagnosticsController(IDiagnosticRepository diagnosticRepository,ApplicationDbContext context,UserManager<ApplicationUser> userManager,IOptions<Pagination> pagination,IUserService userService, IConfiguration _configuration, IHostingEnvironment hostingEnvironment)
        {
            _diagnosticRepository = diagnosticRepository;
            _context = context;
            _userManager = userManager;
            _pagination = pagination.Value;
            _userService = userService;
            Configuration = _configuration;
            _hostingEnv = hostingEnvironment;
        }

        public void Initialize(Diagnostic diagnostic = null)
        {
            var specialities = _context.Specialities.Where(x => x.IsDelete == false).ToList();
            ViewData["Specialities"] = new SelectList(specialities, "Id", "Name");

        }
        public IActionResult Index(int? page = 1, string DiagnosticName = null, int? SpecialityId = null)
        {
            Initialize();
            var diagnostics = _diagnosticRepository.GetAll(DiagnosticName, SpecialityId);
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            return View(diagnostics.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));

        }
        public IActionResult Create()
        {
            Initialize();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Diagnostic diagnostic)
        {
            try
            {
                diagnostic.DoctorId = (int)_userService.Get(User).DoctorId;
                diagnostic = await _userManager.AddUserAndTimestamp(diagnostic, User, DbEnum.DbActionEnum.Create);
                var diagnostics = await _diagnosticRepository.AddAsync(diagnostic);
                if (diagnostics != null)
                {
                    TempData["notice"] = StatusEnum.NoticeStatus.Success;
                    //return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {

            }
            Initialize(diagnostic);
            return View(diagnostic);
        }

        public ActionResult Edit(int id)
        {
            Initialize();
            return View(_diagnosticRepository.Get(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Diagnostic diagnostic)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    diagnostic = await _userManager.AddUserAndTimestamp(diagnostic, User, DbEnum.DbActionEnum.Update);
                    var _speciality = await _diagnosticRepository.UpdateAsync(diagnostic);
                    TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return View();
        }

        public async Task<ActionResult> Delete(int id)
        {
            var _speciality = await _diagnosticRepository.DeleteAsync(id);
            TempData["notice"] = StatusEnum.NoticeStatus.Delete;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult GetAllByDoctor(int id)
        {
            var symptoms = _diagnosticRepository.GetAll(DoctorId: id);
            return Ok(symptoms);
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
            else { 
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
                Diagnostic diagnostic = new Diagnostic();
                diagnostic.DoctorId = (int)_userService.Get(User).DoctorId;
                diagnostic = await _userManager.AddUserAndTimestamp(diagnostic, User, DbEnum.DbActionEnum.Create); 
                dt.Columns.Add("IsDelete", typeof(bool));
                dt.Columns.Add("CreatedAt", typeof(DateTime));
                dt.Columns.Add("CreatedBy", typeof(string));
                dt.Columns.Add("UpdatedAt", typeof(DateTime));
                dt.Columns.Add("UpdatedBy", typeof(string));
                //dt.Columns.Add("SpecialityId", typeof(int));
                dt.Columns.Add("DoctorId", typeof(int));
                foreach (var row in dt.AsEnumerable().ToList())
                {
                    row["IsDelete"] = diagnostic.IsDelete;
                    row["CreatedAt"] = diagnostic.CreatedAt;
                    row["CreatedBy"] = diagnostic.CreatedBy;
                    row["UpdatedAt"] = diagnostic.UpdatedAt;
                    row["UpdatedBy"] = diagnostic.UpdatedBy;
                    //row["SpecialityId"] = diagnostic.SpecialityId;
                    row["DoctorId"] = diagnostic.DoctorId;
                }
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                {
                    //Set the database table name.
                    sqlBulkCopy.DestinationTableName = "dbo.Diagnostic";

                    // Map the Excel columns with that of the database table, this is optional but good if you do
                    // 
                    sqlBulkCopy.ColumnMappings.Add("CreatedAt", "CreatedAt");
                    sqlBulkCopy.ColumnMappings.Add("CreatedBy", "CreatedBy");
                    sqlBulkCopy.ColumnMappings.Add("UpdatedAt", "UpdatedAt");
                    sqlBulkCopy.ColumnMappings.Add("UpdatedBy", "UpdatedBy");
                    sqlBulkCopy.ColumnMappings.Add("IsDelete", "IsDelete");
                    sqlBulkCopy.ColumnMappings.Add("Name", "Name");
                    sqlBulkCopy.ColumnMappings.Add("Description", "Description");
                    //sqlBulkCopy.ColumnMappings.Add("SpecialityId", "SpecialityId");
                    sqlBulkCopy.ColumnMappings.Add("DoctorId", "DoctorId");

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
