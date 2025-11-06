using System;
using System.Collections.Generic;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;

namespace MSIS_HMS.Core.Repositories
{
    public interface IDeliverOrderRepository : IRepository<DeliverOrder>
    {
        List<WarehouseItem> GetDeliverOrderItemsForUpdate(int DeliverOrderId);
        
        List<DeliverOrder> GetAll(int? BranchId, int? DeliverOrderId, int? WarehouseId, int? ItemId, string VoucherNo = null, string Customer = null, DateTime? Date = null, DateTime? StartDate = null, DateTime? EndDate = null);
    }
}