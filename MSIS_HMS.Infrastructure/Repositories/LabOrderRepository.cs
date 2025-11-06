using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Interfaces;
using MSIS_HMS.Infrastructure.Repositories.Base;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class LabOrderRepository : Repository<LabOrder>, ILabOrderRepository
    {
        public LabOrderRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }
        public override List<LabOrder> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabOrders");
            var labOrders = ds.Tables[0].ToList<LabOrder>();
            return labOrders;
        }
        public override List<LabOrder> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabOrders", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var labOrders = ds.Tables[0].ToList<LabOrder>();
            return labOrders;
        }
        public List<LabOrder> GetAll(int? BranchId = null, int? LabOrderId = null, int? PatientId = null, string VoucherNo = null, bool? IsPaid = null, DateTime? Date = null, DateTime? StartDate = null, DateTime? EndDate = null, DateTime? PaidDate = null, DateTime? StartPaidDate = null, DateTime? EndPaidDate = null,int? TestId=null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabOrders", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "LabOrderId", LabOrderId },
                { "PatientId", PatientId },
                { "VoucherNo", VoucherNo },
                { "IsPaid", IsPaid },
                { "Date", Date },
                { "StartDate", StartDate },
                { "EndDate", EndDate },
                { "PaidDate", PaidDate },
                { "StartPaidDate", StartPaidDate },
                { "EndPaidDate", EndPaidDate },
                {"TestId",TestId }
            });
            var labOrders = ds.Tables[0].ToList<LabOrder>();
            return labOrders;
        }
        public override LabOrder Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabOrders", new Dictionary<string, object>() { { "LabOrderId", Id } });
            var labOrders = ds.Tables[0].ToList<LabOrder>();
            var labOrder = labOrders.Count > 0 ? labOrders[0] : null;
            if (labOrder != null)
            {
                labOrder.LabOrderTests = GetLabOrderTests(Id);
            }
            return labOrder;
        }
        public List<LabOrderTest> GetLabOrderTests(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabOrderTests", new Dictionary<string, object>() { { "LabOrderId", Id } });
            var labOrderTests = ds.Tables[0].ToList<LabOrderTest>();
            return labOrderTests;
        }
        public override async Task<LabOrder> UpdateAsync(LabOrder labOrder)
        {
            if (labOrder != null)
            {
                labOrder.LabOrderTests.ToList().ForEach(x => x.LabOrderId = labOrder.Id);
                var existingOrder = _context.LabOrders
                    .Where(p => p.Id == labOrder.Id)
                    .Include(x => x.LabOrderTests)
                    .SingleOrDefault();

                if (existingOrder != null)
                {
                    // Update parent
                    labOrder.CreatedAt = existingOrder.CreatedAt;
                    labOrder.CreatedBy = existingOrder.CreatedBy;
                    _context.Entry(existingOrder).CurrentValues.SetValues(labOrder);

                    // Delete children
                    _context.LabOrderTests.RemoveRange(existingOrder.LabOrderTests);

                    // Insert children
                    existingOrder.LabOrderTests = labOrder.LabOrderTests;

                    await _context.SaveChangesAsync();
                    return labOrder;
                }
            }
            return null;
        }

        public async Task UpdateLabOrderTest(int labOrderId,int labResultId,int labTestId)
        {
            LabOrderTest labOrderTest = new LabOrderTest();
            labOrderTest = _context.LabOrderTests.Where(x => x.LabOrderId == labOrderId && x.LabTestId == labTestId).FirstOrDefault();
            if(labOrderTest!=null)
            {
                labOrderTest.LabResultId = labResultId;
                await _context.SaveChangesAsync();
            }
            
          
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            try
            {
                var labOrder = await _context.LabOrders.FindAsync(Id);
                if (labOrder == null)
                {
                    return false;
                }
                labOrder.IsDelete = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public List<DailyAndMonthlyOrderIncomeDTO> GetIncomeForLabDashboard(int? BranchId = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {

            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetDailyAndMonthlyLabOrderIncome", new Dictionary<string, object>() {
                { "BranchId", BranchId }
            });
            var incomeAmt = ds.Tables[0].ToList<DailyAndMonthlyOrderIncomeDTO>();

            return incomeAmt;
        }
        public List<LabOrder> GetLabOrderResultComplete(int? BranchId=null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_LabOrderResultComplete", new Dictionary<string, object>() {
                { "BranchId", BranchId }
            });
            var completeLabOrder = ds.Tables[0].ToList<LabOrder>();

            return completeLabOrder;
        }
        public List<LabOrder> GetLabOrderFromLabOrderTest(int? BranchId=null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabOrderFromLabOrderTest", new Dictionary<string, object>() {
                { "BranchId", BranchId }
            });
            var completeLabOrder = ds.Tables[0].ToList<LabOrder>();

            return completeLabOrder;
        }
    }
}