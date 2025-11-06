using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Enums;
using MSIS_HMS.Helpers;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Interfaces;
using MSIS_HMS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSIS_HMS.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IIPDRecordRepository _iPDRecordRepository;
        private readonly IBranchService _branchService;
        public PaymentsController(IPaymentRepository paymentRepository, ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment, IIPDRecordRepository iPDRecordRepository, IBranchService branchService)
        {
            _paymentRepository = paymentRepository;
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _iPDRecordRepository = iPDRecordRepository;
            _branchService = branchService;

        }
        public void Initialize(IPDPayment iPDPayment = null, int? RecordId = null)
        {


        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create(int ipdRecordId)
        {
            TempData.Remove("date");

            IPDPayment ipdPayment = new IPDPayment();
            ipdPayment.IPDRecordId = ipdRecordId;
            var iPDPayments = _paymentRepository.GetAll(ipdRecordId);
            ViewData["Payments"] = iPDPayments;
            var paymentDetailDTOs = _paymentRepository.GetPaymentDetail(ipdRecordId);
            ViewData["PaymentDetail"] = paymentDetailDTOs;
            var ipdRecordData = _iPDRecordRepository.Get(ipdRecordId);
            if(ipdRecordData!=null)
            {
                ViewData["Discount"] = ipdRecordData.Discount;
                ViewData["Tax"] = ipdRecordData.Tax;
            }
            else
            {
                ViewData["Discount"] =0.00;
                ViewData["Tax"] = 0.00;
            }
            TempData["ipdRecordId"] = ipdRecordId;
            return View(ipdPayment);
        }
        [HttpPost]
        public async Task<IActionResult> Create(IPDPayment iPDPayment)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    if (iPDPayment.Id == 0)
                    {
                        iPDPayment = await _userManager.AddUserAndTimestamp(iPDPayment, User, DbEnum.DbActionEnum.Create);

                        var _ipdPayment = await _paymentRepository.AddAsync(iPDPayment);
                        if (_ipdPayment != null)
                        {
                            TempData["notice"] = StatusEnum.NoticeStatus.Success;

                            //_logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));                   
                            return RedirectToAction("Create", new { ipdRecordId = iPDPayment.IPDRecordId });
                        }
                    }
                    else
                    {
                        iPDPayment = await _userManager.AddUserAndTimestamp(iPDPayment, User, DbEnum.DbActionEnum.Update);
                        var _appointmentType = await _paymentRepository.UpdateAsync(iPDPayment);
                        if (_appointmentType != null)
                        {
                            TempData["notice"] = StatusEnum.NoticeStatus.Success;
                            //_logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));
                            return RedirectToAction("Create", new { ipdRecordId = iPDPayment.IPDRecordId });
                        }
                    }
                    //}
                    //else
                    //{
                    //    return RedirectToAction(nameof(Create));
                    //}

                }

            }
            catch (Exception ex)
            {

            }
            return View();
        }
        [HttpGet]
        public IActionResult GetRecordByRecordId(int? RecordId)
        {
            var ipdRecord = _paymentRepository.GetRecordByRecordId(RecordId);
            ipdRecord.Image = ipdRecord.Image.GetBase64();
            return Ok(ipdRecord);
        }
        [HttpGet]
        public IActionResult GetPaymentById(int id)
        {
            var payment = _paymentRepository.Get(id);
            return Ok(payment);
        }
        public async Task<IActionResult> Paid()
        {
            int IPDRecordId =(int) TempData["ipdRecordId"] ;
            var ipdrecord = await _context.IPDRecords.FindAsync(IPDRecordId);
            if (ipdrecord != null)
            {
                ipdrecord.IsPaid = true;
                ipdrecord.PaidDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            else
            {
                TempData["notice"] = StatusEnum.NoticeStatus.Fail;
            }
            return RedirectToAction(nameof(Create), new { ipdRecordId = IPDRecordId } );
        }
        public async Task<ActionResult> Delete(int id)
        {
            var payment = _paymentRepository.Get(id);
            try
            {
                var _ipdPayment = await _paymentRepository.DeleteAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
                //_logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Delete));
            }
            catch (Exception e)
            {
                //_logger.LogError(e.InnerException.Message);
            }

            return RedirectToAction("Create", new { ipdRecordId = payment.IPDRecordId });
        }
        public ActionResult Image(string path)
        {
            return File(FtpHelper.DownloadFileFromServer(path), "image/png");
        }

        public IActionResult GetIPDRecordDetailByRecordId(int ipdRecordId, DateTime date)
        {
            TempData.Remove("date");
            var iPDRecordDetail = _paymentRepository.GetIPDRecordDetailByRecordId(ipdRecordId, date);
            // ViewData["iPDRecordDetail"] = iPDRecordDetail;
            TempData["date"] = date;
            return Ok(iPDRecordDetail);
        }

        public IActionResult PrintAmountReport(int ipdRecordId)
        {
            
            string renderFormat = "PDF";
            string extension = "pdf";
            string mimetype = "application/pdf";
            using (var report = new LocalReport())
            {
                List<PaymentAmountDTO> paymentDetailDTOs = new List<PaymentAmountDTO>();
                List<Branch> branches = new List<Branch>();
                paymentDetailDTOs = _paymentRepository.GetPaymentAmount(ipdRecordId);
                var ipdRecord = _iPDRecordRepository.Get(ipdRecordId);
                decimal subTotal = 0;
                foreach (var sub in paymentDetailDTOs)
                {
                    subTotal += sub.Amount;
                    sub.Discount = ipdRecord.Discount;
                    sub.Tax = ipdRecord.Tax;
                }
                var branch = _branchService.GetBranchById(ipdRecord.BranchId);
                branches.Add(branch);
                report.DataSources.Add(new ReportDataSource("DataSet1", paymentDetailDTOs));
                report.DataSources.Add(new ReportDataSource("DataSet2", branches));
                report.ReportPath = $"{_webHostEnvironment.WebRootPath}\\ReportFiles\\PaymentAmountReport.rdlc";
                
                var pdf = report.Render(renderFormat);
                return File(pdf, mimetype, ipdRecord.Patient.Name+"_Total_"+DateTime.Now+"." + extension);
            }

        }
        [HttpGet]
        public IActionResult PrintReport(int ipdRecordId)
        {
            DateTime date = Convert.ToDateTime(TempData["date"].ToString());
            string renderFormat = "PDF";
            string extension = "pdf";
            string mimetype = "application/pdf";
            using (var report = new LocalReport())
            {
                List<IPDRecordDetailReportDTO> paymentDetailDTOs = new List<IPDRecordDetailReportDTO>();
                List<Branch> branches = new List<Branch>();
                paymentDetailDTOs = _paymentRepository.GetIPDRecordDetailForReport(ipdRecordId, date);
                var ipdRecordInfo = _iPDRecordRepository.Get(ipdRecordId);
                var branch = _branchService.GetBranchById(ipdRecordInfo.BranchId);

                branches.Add(branch);

                report.DataSources.Add(new ReportDataSource("DataSet1", paymentDetailDTOs));
                report.DataSources.Add(new ReportDataSource("Branch", branches));
                report.ReportPath = $"{_webHostEnvironment.WebRootPath}\\ReportFiles\\IPDRecordDetailReport.rdlc";

                var pdf = report.Render(renderFormat);
                TempData["date"] = date;
                return File(pdf, mimetype, ipdRecordInfo.Patient.Name + "_DailyCharges_"+DateTime.Now+"." + extension);
            }

        }
      
       public async Task<IActionResult> DiscountAndTax(decimal discount,decimal tax,int ipdRecordId)
       {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    IPDRecord iPDRecord = new IPDRecord();
                    iPDRecord = _iPDRecordRepository.Get(ipdRecordId);
                    iPDRecord.Discount = discount;
                    iPDRecord.Tax = tax;
                    iPDRecord = await _userManager.AddUserAndTimestamp(iPDRecord, User, DbEnum.DbActionEnum.Update);
                    var _ipdRecord = await _iPDRecordRepository.UpdateAsync(iPDRecord);
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {

                }
            }
           
            return Ok();

       }
    }
}
