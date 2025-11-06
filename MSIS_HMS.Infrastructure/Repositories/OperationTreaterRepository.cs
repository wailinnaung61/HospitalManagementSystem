using Microsoft.EntityFrameworkCore;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Interfaces;
using MSIS_HMS.Infrastructure.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class OperationTreaterRepository: Repository<OperationTreater>, IOperationTreaterRepository
    {
        public OperationTreaterRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }
        public List<OperationTreater> GetAll(int? BranchId = null, int? OutletId = null, int? OrderId = null, int? PatientId = null, int? DoctorId = null, string VoucherNo = null, bool? IsPaid = null, DateTime? Date = null, DateTime? StartDate = null, DateTime? EndDate = null, DateTime? PaidDate = null, DateTime? StartPaidDate = null, DateTime? EndPaidDate = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOperationTreaters", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "OutletId", OutletId },
                { "OperationTreaterId", OrderId },
                { "PatientId", PatientId },
                { "DoctorId", DoctorId },
                { "IsPaid", IsPaid },
                { "Date", Date },
                { "StartDate", StartDate },
                { "EndDate", EndDate }
            });
            var orders = ds.Tables[0].ToList<OperationTreater>();
            orders.ForEach(x => x.OT_Doctors = GetOTDoctors(x.Id));
            orders.ForEach(x => x.OT_Staffs = GetOTStaffs(x.Id));
            orders.ForEach(x => x.OperationItems = GetOperationItems(x.Id));
            orders.ForEach(x => x.OperationServices = GetOperationService(x.Id));
            orders.ForEach(x => x.OT_Anaesthetists = GetOTAnaesthetists(x.Id));
          
            return orders;
        }
        
        private List<OT_Doctor> GetOTDoctors(int OTID)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOTDoctors", new Dictionary<string, object>() { { "OperationTreaterId", OTID } });
            return ds.Tables[0].ToList<OT_Doctor>();
        }
        private List<OT_Anaesthetist> GetOTAnaesthetists(int OTID)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOTAnaesthetists", new Dictionary<string, object>() { { "OperationTreaterId", OTID } });
            return ds.Tables[0].ToList<OT_Anaesthetist>();
        }
        private List<OT_Staff> GetOTStaffs(int OTID)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOTStaffs", new Dictionary<string, object>() { { "OperationTreaterId", OTID } });
            return ds.Tables[0].ToList<OT_Staff>();
        }
       
        public override OperationTreater Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOperationTreaters", new Dictionary<string, object>() { { "OperationTreaterId", Id } });
            var operationTreaters = ds.Tables[0].ToList<OperationTreater>();
            var operationTreater = operationTreaters.Count > 0 ? operationTreaters[0] : null;
            if (operationTreater != null)
            {
                operationTreater.OperationItems = GetOperationItems(Id);
                operationTreater.OperationServices = GetOperationService(Id);
                operationTreater.OT_Doctors = GetOTDoctors(Id);
                operationTreater.OT_Staffs = GetOTStaffs(Id);
                operationTreater.OT_Anaesthetists = GetOTAnaesthetists(Id);
            }
            return operationTreater;
        }
        public List<OperationItem> GetOperationItems(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOperationItems", new Dictionary<string, object>() { { "OperationTreaterId", Id } });
            var operationItems = ds.Tables[0].ToList<OperationItem>();
            return operationItems;
        }
        public List<OperationService> GetOperationService(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOperationServices", new Dictionary<string, object>() { { "OperationTreaterId", Id } });
            var operationServices = ds.Tables[0].ToList<OperationService>();
            return operationServices;
        }
        public decimal GetIncomeForOT(int? BranchId = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            decimal dailyTotalAmt = 0;
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOperationThreaterIncome", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "StartDate", StartDate },
                { "EndDate", EndDate }
            });
            var incomeAmt = ds.Tables[0].ToList<DailyIncomeDTO>();
            foreach (var amt in incomeAmt)
            {
                dailyTotalAmt = amt.OTIncome;
            }
            return dailyTotalAmt;
        }
        public List<OutletItem> GetOrderItemsForUpdate(int orderId, int outletId)
        {
            var operationItems = GetOperationItems(orderId);
            return operationItems.Select(x => new OutletItem
            {
                OutletId = outletId,
                ItemId = x.ItemId,
                Qty = x.QtyInSmallestUnit
            }).GroupBy(x => new { x.OutletId, x.ItemId }, (key, g) => new OutletItem
            {
                OutletId = outletId,
                ItemId = g.First().ItemId,
                Qty = g.Sum(i => i.Qty)
            }).ToList();
        }
        public override async Task<OperationTreater> UpdateAsync(OperationTreater operationTreater)
        {
            if (operationTreater != null)
            {
                if(operationTreater.OperationItems!=null || operationTreater.OperationServices!=null)
                {
                    operationTreater.OperationItems.ToList().ForEach(x => x.OperationTreaterId = operationTreater.Id);
                    var existingOrder = _context.OperationTreaters
                        .Where(p => p.Id == operationTreater.Id)
                        .Include(p => p.OperationItems)
                        .Include(x => x.OperationServices)
                        .Include(x => x.OT_Doctors)
                        .Include(x => x.OT_Staffs)
                        .Include(x=>x.OT_Anaesthetists)
                        .SingleOrDefault();

                    if (existingOrder != null)
                    {
                        // Update parent
                        _context.Entry(existingOrder).CurrentValues.SetValues(operationTreater);

                        // Delete children
                        _context.OperationItems.RemoveRange(existingOrder.OperationItems);
                        _context.OperationServices.RemoveRange(existingOrder.OperationServices);
                        _context.OT_Doctors.RemoveRange(existingOrder.OT_Doctors);
                        _context.OT_Staffs.RemoveRange(existingOrder.OT_Staffs);
                        _context.OT_Anaesthetists.RemoveRange(existingOrder.OT_Anaesthetists);

                        // Insert children
                        existingOrder.OperationItems = operationTreater.OperationItems;
                        existingOrder.OperationServices = operationTreater.OperationServices;
                        existingOrder.OT_Doctors = operationTreater.OT_Doctors;
                        existingOrder.OT_Staffs = operationTreater.OT_Staffs;
                        existingOrder.OT_Anaesthetists = operationTreater.OT_Anaesthetists;

                        await _context.SaveChangesAsync();
                        return operationTreater;
                    }
                }
                else
                {
                   
                    var existingOrder = _context.OperationTreaters
                        .Where(p => p.Id == operationTreater.Id)                       
                        .Include(x => x.OT_Doctors)
                        .Include(x => x.OT_Staffs)
                        .Include(x => x.OT_Anaesthetists)
                        .SingleOrDefault();                  

                    if (existingOrder != null)
                    {
                        // Update parent
                        _context.Entry(existingOrder).CurrentValues.SetValues(operationTreater);

                        // Delete children                       
                        _context.OT_Doctors.RemoveRange(existingOrder.OT_Doctors);
                        _context.OT_Staffs.RemoveRange(existingOrder.OT_Staffs);
                        _context.OT_Anaesthetists.RemoveRange(existingOrder.OT_Anaesthetists);

                        // Insert children
                        existingOrder.OT_Doctors = operationTreater.OT_Doctors;
                        existingOrder.OT_Staffs = operationTreater.OT_Staffs;
                        existingOrder.OT_Anaesthetists = operationTreater.OT_Anaesthetists;

                        await _context.SaveChangesAsync();
                        return operationTreater;
                    }
                }
               
            }
            return null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            try
            {
                var order = await _context.OperationTreaters.FindAsync(Id);
                if (order == null)
                {
                    return false;
                }
                order.IsDelete = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
