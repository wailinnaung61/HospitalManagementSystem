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
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }
        public override List<Order> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOrders");
            var orders = ds.Tables[0].ToList<Order>();
            return orders;
        }
        public override List<Order> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOrders", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var orders = ds.Tables[0].ToList<Order>();
            return orders;
        }
        public List<Order> GetAll(int? BranchId, int? OutletId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOrders", new Dictionary<string, object>() { { "BranchId", BranchId }, { "OutletId", OutletId } });
            var orders = ds.Tables[0].ToList<Order>();
            return orders;
        }
        public List<Order> GetAll(int? BranchId = null, int? OutletId = null, int? OrderId = null, int? PatientId = null, int? DoctorId = null, string VoucherNo = null, bool? IsPaid = null, DateTime? Date = null, DateTime? StartDate = null, DateTime? EndDate = null, DateTime? PaidDate = null, DateTime? StartPaidDate = null, DateTime? EndPaidDate = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOrders", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "OutletId", OutletId },
                { "OrderId", OrderId },
                { "PatientId", PatientId },
                { "DoctorId", DoctorId },
                { "VoucherNo", VoucherNo },
                { "IsPaid", IsPaid },
                { "Date", Date },
                { "StartDate", StartDate },
                { "EndDate", EndDate },
                { "PaidDate", PaidDate },
                { "StartPaidDate", StartPaidDate },
                { "EndPaidDate", EndPaidDate },
            });
            var orders = ds.Tables[0].ToList<Order>();
            return orders;
        }
        public override Order Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOrders", new Dictionary<string, object>() { { "OrderId", Id } });
            var orders = ds.Tables[0].ToList<Order>();
            var order = orders.Count > 0 ? orders[0] : null;
            if (order != null)
            {
                order.OrderItems = GetOrderItems(Id);
                order.OrderServices = GetOrderServices(Id);
            }
            return order;
        }
        public List<OrderItem> GetOrderItems(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOrderItems", new Dictionary<string, object>() { { "OrderId", Id } });
            var orderItems = ds.Tables[0].ToList<OrderItem>();
            return orderItems;
        }
        public List<OrderService> GetOrderServices(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOrderServices", new Dictionary<string, object>() { { "OrderId", Id } });
            var orderServices = ds.Tables[0].ToList<OrderService>();
            return orderServices;
        }
        public List<OutletItem> GetOrderItemsForUpdate(int orderId, int outletId)
        {
            var orderItems = GetOrderItems(orderId);
            return orderItems.Select(x => new OutletItem
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
        public override async Task<Order> UpdateAsync(Order order)
        {
            if (order != null)
            {
                order.OrderItems.ToList().ForEach(x => x.OrderId = order.Id);
                var existingOrder = _context.Orders
                    .Where(p => p.Id == order.Id)
                    .Include(p => p.OrderItems)
                    .Include(x => x.OrderServices)
                    .SingleOrDefault();

                if (existingOrder != null)
                {
                    // Update parent
                    order.CreatedAt = existingOrder.CreatedAt;
                    order.CreatedBy = existingOrder.CreatedBy;
                    _context.Entry(existingOrder).CurrentValues.SetValues(order);

                    // Delete children
                    _context.OrderItems.RemoveRange(existingOrder.OrderItems);
                    _context.OrderServices.RemoveRange(existingOrder.OrderServices);

                    // Insert children
                    existingOrder.OrderItems = order.OrderItems;
                    existingOrder.OrderServices = order.OrderServices;

                    await _context.SaveChangesAsync();
                    return order;
                }
            }
            return null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(Id);
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

        public decimal GetIncome(int? BranchId = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            decimal dailyTotalAmt = 0;
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetDailyIncome", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "StartDate", StartDate },
                { "EndDate", EndDate }              
            });
            var incomeAmt = ds.Tables[0].ToList<DailyIncomeDTO>();
            foreach(var amt in incomeAmt)
            {
                dailyTotalAmt = amt.OrderIncome + amt.LabOrderIncome + amt.IPDIncome + amt.OTIncome + amt.OPDIncome;
            }
            return dailyTotalAmt;
        }
        public List<DailyAndMonthlyIncomeDTO> GetIncomeForDailyandMonthly(int? BranchId=null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            //List<DailyAndMonthlyIncomeDTO> dailyAndMonthly = new List<DailyAndMonthlyIncomeDTO>();
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetDailyAndMonthlyIncome", new Dictionary<string, object>() {
                { "BranchId", BranchId }
                
            });
            var incomeAmtForDaily = ds.Tables[0].ToList<DailyAndMonthlyIncomeDTO>();

            return incomeAmtForDaily;

        }
        public List<DailyAndMonthlyOrderIncomeDTO> GetIncomeForPharmacyDashboard(int? BranchId = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetDailyAndMonthlyOrderIncome", new Dictionary<string, object>() {
                { "BranchId", BranchId }
            });
            var incomeAmt = ds.Tables[0].ToList<DailyAndMonthlyOrderIncomeDTO>();
           
            return incomeAmt;
        }
        public List<DailyAndMonthlyOutletOrderIncomeDTO> GetOutletIncomeForPharmacyDashboard(int? BranchId = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetOutletOrderIncome", new Dictionary<string, object>() {
                { "BranchId", BranchId }
            });
            var incomeAmt = ds.Tables[0].ToList<DailyAndMonthlyOutletOrderIncomeDTO>();
            int i = 0;
            foreach (var val in incomeAmt)
            {
                val.No = ++i;
                val.DailyOrder = _context.Orders.Include(x => x.Outlet).Where(x => x.Date.Date == DateTime.Now.Date && x.OutletId==val.OutletId && x.IsDelete==false).ToList().Count;
            }
            return incomeAmt;
        }


    }
}