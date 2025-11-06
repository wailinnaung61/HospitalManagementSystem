using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Enums;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Interfaces;
using MSIS_HMS.Infrastructure.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class PaymentRepository : Repository<IPDPayment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }
        public override List<IPDPayment> GetAll(int? RecordId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetPayments", new Dictionary<string, object>
            { { "RecordId",RecordId} }
            );
            var ipdPayments = ds.Tables[0].ToList<IPDPayment>();
            return ipdPayments;
        }
        public override IPDPayment Get(int id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetPayments", new Dictionary<string, object>
            { { "Id",id} }
            );
            var ipdPayments = ds.Tables[0].ToList<IPDPayment>();
            return ipdPayments.Count > 0 ? ipdPayments[0] : null;
        }
        public PaymentCommonDTO GetRecordByRecordId(int? RecordId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetIPDRecordByRecordId", new Dictionary<string, object>()
            { { "RecordId", RecordId } });
            var records = ds.Tables[0].ToList<PaymentCommonDTO>();
            if (records[0] != null)
            {
                if (records[0].PaymentType == Core.Enums.PaymentTypeEnum.Advance)
                {
                    records[0].PaymentTypeName = Core.Enums.PaymentTypeEnum.Advance.ToDescription();
                }
                else if (records[0].PaymentType == Core.Enums.PaymentTypeEnum.Cash)
                {
                    records[0].PaymentTypeName = Core.Enums.PaymentTypeEnum.Cash.ToDescription();
                }
                else
                {
                    records[0].PaymentTypeName = Core.Enums.PaymentTypeEnum.Credit.ToDescription();
                }

            }
            return records.Count > 0 ? records[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var payment = await _context.IPDPayments.FindAsync(Id);
                    if (payment == null)
                    {
                        return false;
                    }
                    payment.IsDelete = true;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (DbException e)
                {
                    Console.WriteLine(e.Message);
                    await transaction.RollbackAsync();
                }
            }
            return false;
        }

        public List<PaymentDetailDTO> GetPaymentDetail(int? RecordId)
        {
            List<PaymentDetailDTO> lstiPDPaymentDTOs = new List<PaymentDetailDTO>();
            var ipdRecord = GetRecordByRecordId(RecordId);
            var patientInfo = _context.Patients.Find(ipdRecord.PatientId);
            var branchInfo = _context.Branches.Find(patientInfo.BranchId);
            int branchCheckIn = branchInfo.CheckInTime.Hours; 
            int branchCheckout = branchInfo.CheckOutTime.Hours;
            List<DateTime> dateTimes = new List<DateTime>();
            List<int> days = new List<int>();
            if (ipdRecord.DODC == Convert.ToDateTime("1/1/0001 12:00:00 AM") || ipdRecord.DODC==null)
            {
                var curdate = DateTime.Now.Date;
                var setdate = new DateTime();
                if (ipdRecord.DOA.Hour < branchCheckIn)
                {
                    var doa = ipdRecord.DOA.Date.AddDays(-1);
                    ipdRecord.DOA= new DateTime(doa.Year, doa.Month, doa.Day, ipdRecord.DOA.Hour, ipdRecord.DOA.Minute, ipdRecord.DOA.Second);
                    setdate = ipdRecord.DOA;
                    dateTimes.Add(setdate);
                }
                else
                {
                    setdate = ipdRecord.DOA.Date;
                    dateTimes.Add(setdate);
                }
                days.Add(0);
                while (setdate != curdate)
                {


                    var h = new DateTime(ipdRecord.DOA.Year, ipdRecord.DOA.Month, ipdRecord.DOA.Day, branchCheckIn, 0, 0);
                    setdate = setdate.AddDays(1).Date;
                    var ch = new DateTime(setdate.Year, setdate.Month, setdate.Day, branchCheckout, 0, 0);
                    TimeSpan diffT = ch - h;
                    days.Add(diffT.Days);

                    //setdate = setdate.AddDays(1).Date;
                    dateTimes.Add(setdate);


                }


            }
            else
            {
                var curdate = ipdRecord.DODC.Date;
                DateTime setdate =new DateTime();
                if (ipdRecord.DOA.Hour < branchCheckIn)
                {
                    var doa = ipdRecord.DOA.Date.AddDays(-1);
                    ipdRecord.DOA = new DateTime(doa.Year, doa.Month, doa.Day, ipdRecord.DOA.Hour, ipdRecord.DOA.Minute, ipdRecord.DOA.Second);
                    setdate = ipdRecord.DOA;
                    dateTimes.Add(setdate);
                }
                else
                {
                    setdate = ipdRecord.DOA.Date;
                    dateTimes.Add(setdate);
                }
                days.Add(0);
                
                while (setdate != curdate)
                {
                    var h = new DateTime(ipdRecord.DOA.Year, ipdRecord.DOA.Month, ipdRecord.DOA.Day, branchCheckIn, 0, 0);
                    setdate = setdate.AddDays(1).Date;
                    var ch = new DateTime(setdate.Year, setdate.Month, setdate.Day, branchCheckout, 0, 0);
                    TimeSpan diffT = ch - h;
                    days.Add(diffT.Days);                   
                    dateTimes.Add(setdate);
                }
            }
            int i = 0;
            foreach (var d in dateTimes)
            {
                DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetPaymentAmountBydate", new Dictionary<string, object>()
                { { "date", d },
                  { "IPDRecordId",RecordId} });
                var iPDPaymentDTOs = ds.Tables[0].ToList<PaymentDetailDTO>();
                var iPDPaymentDTO = iPDPaymentDTOs.Count > 0 ? iPDPaymentDTOs[0] : new PaymentDetailDTO();
                
                iPDPaymentDTO.Day = days[i];
                iPDPaymentDTO.Date = d;
                iPDPaymentDTO.Total = iPDPaymentDTO.RoomCharges + iPDPaymentDTO.Services + iPDPaymentDTO.Food + iPDPaymentDTO.Medications + iPDPaymentDTO.Fees;
                lstiPDPaymentDTOs.Add(iPDPaymentDTO);
                i++;
            }
            return lstiPDPaymentDTOs;
        }

        public List<PaymentAmountDTO> GetPaymentAmount(int ipdRecordId)
        {
            PaymentAmountDTO paymentDetailDTO = new PaymentAmountDTO();
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetPaymentAmount", new Dictionary<string, object>()
                { { "IPDRecordId",ipdRecordId} });
            var iPDPaymentDTOs = ds.Tables[0].ToList<PaymentAmountDTO>();
           
            //var iPDPaymentDTO = iPDPaymentDTOs.Count > 0 ? iPDPaymentDTOs[0] : new PaymentAmountDTO();
            return iPDPaymentDTOs;
        }

        public IPDRecordDetailDTO GetIPDRecordDetailByRecordId(int RecordId, DateTime date)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetIPDRecordDetailByRecordId", new Dictionary<string, object>()
                { { "Date", date },
                  { "RecordId",RecordId} });
            var roomChargesDTOs = ds.Tables[0].ToList<RoomChargesDTO>();
            var medicationDTOs = ds.Tables[1].ToList<MedicationsDTO>();
            var serviceDTOs = ds.Tables[2].ToList<IPDOrderServiceDTO>();
            var feesDTOs = ds.Tables[3].ToList<FeesDTO>();
            var foodDTOs = ds.Tables[4].ToList<FoodDTO>();
            IPDRecordDetailDTO iPDRecordDetailDTO = new IPDRecordDetailDTO();
            iPDRecordDetailDTO.roomChargesDTOs = roomChargesDTOs;
            iPDRecordDetailDTO.medicationsDTOs = medicationDTOs;
            iPDRecordDetailDTO.iPDOrderServiceDTOs = serviceDTOs;
            iPDRecordDetailDTO.feesDTOs = feesDTOs;
            iPDRecordDetailDTO.foodDTOs = foodDTOs;
            return iPDRecordDetailDTO;
        }
        public List<IPDRecordDetailReportDTO> GetIPDRecordDetailForReport(int RecordId, DateTime date)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetIPDRecordDetailByRecordId", new Dictionary<string, object>()
                { { "Date", date },
                  { "RecordId",RecordId} });
            var roomChargesDTOs = ds.Tables[0].ToList<RoomChargesDTO>();
            var medicationDTOs = ds.Tables[1].ToList<MedicationsDTO>();
            var serviceDTOs = ds.Tables[2].ToList<IPDOrderServiceDTO>();
            var feesDTOs = ds.Tables[3].ToList<FeesDTO>();
            var foodDTOs = ds.Tables[4].ToList<FoodDTO>();
            decimal subtotal = 0;
            int i = 0;
            List<IPDRecordDetailReportDTO> iPDRecordDetailReportDTOs = new List<IPDRecordDetailReportDTO>();
            if (roomChargesDTOs.Count > 0)
            {
                IPDRecordDetailReportDTO roomCharges = new IPDRecordDetailReportDTO();
                roomCharges.Name = "Room Charges";
                iPDRecordDetailReportDTOs.Add(roomCharges);
                foreach (var rc in roomChargesDTOs)
                {
                    i += 1;
                    IPDRecordDetailReportDTO iPDRecordDetailReportDTO = new IPDRecordDetailReportDTO();
                    iPDRecordDetailReportDTO.No = i;
                    iPDRecordDetailReportDTO.Name = rc.RoomName + "/" + rc.BedName;
                    iPDRecordDetailReportDTO.UnitPrice = rc.UnitPrice;
                    iPDRecordDetailReportDTO.Qty = rc.Qty.ToString();
                    iPDRecordDetailReportDTO.Amount = rc.UnitPrice * rc.Qty;
                    iPDRecordDetailReportDTOs.Add(iPDRecordDetailReportDTO);
                    
                    subtotal += iPDRecordDetailReportDTO.Amount;
                    
                }
                IPDRecordDetailReportDTO roomChargesSubTotal = new IPDRecordDetailReportDTO();
                roomChargesSubTotal.Qty = "SubTotal";
                roomChargesSubTotal.Amount = subtotal;
                iPDRecordDetailReportDTOs.Add(roomChargesSubTotal);
            }
            if (medicationDTOs.Count > 0)
            {
                subtotal = 0;
                i = 0;
                IPDRecordDetailReportDTO medication = new IPDRecordDetailReportDTO();
                medication.Name = "Medication";
                iPDRecordDetailReportDTOs.Add(medication);
                foreach (var m in medicationDTOs)
                {
                    i += 1;
                    IPDRecordDetailReportDTO iPDRecordDetailReportDTO = new IPDRecordDetailReportDTO();
                    iPDRecordDetailReportDTO.No = i;
                    iPDRecordDetailReportDTO.Name = m.Name;
                    iPDRecordDetailReportDTO.UnitPrice = m.UnitPrice;
                    iPDRecordDetailReportDTO.Qty = m.Qty.ToString();
                    iPDRecordDetailReportDTO.UnitName = m.UnitName;
                    iPDRecordDetailReportDTO.Amount = m.UnitPrice * m.Qty;
                    iPDRecordDetailReportDTOs.Add(iPDRecordDetailReportDTO);
                    subtotal += iPDRecordDetailReportDTO.Amount;
                }

                IPDRecordDetailReportDTO medicationSubTotal = new IPDRecordDetailReportDTO();
                medicationSubTotal.Qty = "SubTotal";
                medicationSubTotal.Amount = subtotal;
                iPDRecordDetailReportDTOs.Add(medicationSubTotal);
            }
            if (serviceDTOs.Count > 0)
            {
                i = 0;
                subtotal = 0;
                IPDRecordDetailReportDTO services = new IPDRecordDetailReportDTO();
                services.Name = "Service";
                iPDRecordDetailReportDTOs.Add(services);
                foreach (var s in serviceDTOs)
                {
                    i += 1;
                    IPDRecordDetailReportDTO iPDRecordDetailReportDTO = new IPDRecordDetailReportDTO();
                    iPDRecordDetailReportDTO.No = i;
                    iPDRecordDetailReportDTO.Name = s.Name;
                    iPDRecordDetailReportDTO.UnitPrice = s.UnitPrice;
                    iPDRecordDetailReportDTO.Qty = s.Qty.ToString();
                    iPDRecordDetailReportDTO.Amount = s.UnitPrice * s.Qty;
                    iPDRecordDetailReportDTOs.Add(iPDRecordDetailReportDTO);
                    subtotal += iPDRecordDetailReportDTO.Amount;
                }

                IPDRecordDetailReportDTO servicesSubTotal = new IPDRecordDetailReportDTO();
                servicesSubTotal.Qty = "SubTotal";
                servicesSubTotal.Amount = subtotal;
                iPDRecordDetailReportDTOs.Add(servicesSubTotal);
            }
            if (feesDTOs.Count > 0)
            {
                i = 0;
                subtotal = 0;
                IPDRecordDetailReportDTO fees = new IPDRecordDetailReportDTO();
                fees.Name = "Round & Other Fees";
                iPDRecordDetailReportDTOs.Add(fees);
                foreach (var fe in feesDTOs)
                {
                    i += 1;
                    IPDRecordDetailReportDTO iPDRecordDetailReportDTO = new IPDRecordDetailReportDTO();
                    iPDRecordDetailReportDTO.No = i;
                    iPDRecordDetailReportDTO.Name = fe.FeesName;
                    iPDRecordDetailReportDTO.UnitPrice = fe.UnitPrice;
                    iPDRecordDetailReportDTO.Qty = fe.Qty.ToString();
                    iPDRecordDetailReportDTO.Amount = fe.UnitPrice * fe.Qty;
                    iPDRecordDetailReportDTOs.Add(iPDRecordDetailReportDTO);
                    subtotal += iPDRecordDetailReportDTO.Amount;
                }

                IPDRecordDetailReportDTO feesSubTotal = new IPDRecordDetailReportDTO();
                feesSubTotal.Qty = "SubTotal";
                feesSubTotal.Amount = subtotal;
                iPDRecordDetailReportDTOs.Add(feesSubTotal);
            }
            if (foodDTOs.Count > 0)
            {
                i = 0;
                subtotal = 0;
                IPDRecordDetailReportDTO foods = new IPDRecordDetailReportDTO();
                foods.Name = "Food";
                iPDRecordDetailReportDTOs.Add(foods);
                foreach (var f in foodDTOs)
                {
                    i += 1;
                    IPDRecordDetailReportDTO iPDRecordDetailReportDTO = new IPDRecordDetailReportDTO();
                    iPDRecordDetailReportDTO.No = i;
                    iPDRecordDetailReportDTO.Name = f.Name;
                    iPDRecordDetailReportDTO.UnitPrice = f.UnitPrice;
                    iPDRecordDetailReportDTO.Qty = f.Qty.ToString();
                    iPDRecordDetailReportDTO.Amount = f.UnitPrice * f.Qty;
                    iPDRecordDetailReportDTOs.Add(iPDRecordDetailReportDTO);
                    subtotal += iPDRecordDetailReportDTO.Amount;
                }

                IPDRecordDetailReportDTO foodsSubTotal = new IPDRecordDetailReportDTO();
                foodsSubTotal.Qty = "SubTotal";
                foodsSubTotal.Amount = subtotal;
                iPDRecordDetailReportDTOs.Add(foodsSubTotal);
            }
            i = 0;
            return iPDRecordDetailReportDTOs;
        }


    }
}
