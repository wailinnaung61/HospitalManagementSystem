using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Repositories
{
    public interface IOperationTreaterRepository : IRepository<OperationTreater>
    {
        List<OperationTreater> GetAll(int? BranchId = null, int? OutletId = null, int? OrderId = null, int? PatientId = null, int? DoctorId = null, string VoucherNo = null, bool? IsPaid = null, DateTime? Date = null, DateTime? StartDate = null, DateTime? EndDate = null, DateTime? PaidDate = null, DateTime? StartPaidDate = null, DateTime? EndPaidDate = null);
        List<OutletItem> GetOrderItemsForUpdate(int orderId, int outletId);
        decimal GetIncomeForOT(int? BranchId = null, DateTime? StartDate = null, DateTime? EndDate = null);
    }
}
