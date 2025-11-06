using System;
using System.Collections.Generic;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Repositories.Base;

namespace MSIS_HMS.Core.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        List<Order> GetAll(int? BranchId, int? OutletId);
        List<Order> GetAll(int? BranchId = null, int? OutletId = null, int? OrderId = null, int? PatientId = null, int? DoctorId = null, string VoucherNo = null, bool? IsPaid = null, DateTime? Date = null, DateTime? StartDate = null, DateTime? EndDate = null, DateTime? PaidDate = null, DateTime? StartPaidDate = null, DateTime? EndPaidDate = null);
        List<OrderItem> GetOrderItems(int Id);
        List<OrderService> GetOrderServices(int Id);
        List<OutletItem> GetOrderItemsForUpdate(int orderId, int outletId);
        decimal GetIncome(int? BranchId = null, DateTime? StartDate = null, DateTime? EndDate = null);
        List<DailyAndMonthlyIncomeDTO> GetIncomeForDailyandMonthly(int? BranchId = null, DateTime? StartDate = null, DateTime? EndDate = null);
        List<DailyAndMonthlyOrderIncomeDTO> GetIncomeForPharmacyDashboard(int? BranchId = null, DateTime? StartDate = null, DateTime? EndDate = null);
        List<DailyAndMonthlyOutletOrderIncomeDTO> GetOutletIncomeForPharmacyDashboard(int? BranchId = null, DateTime? StartDate = null, DateTime? EndDate = null);
    }
}
