using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Repositories
{
    public interface IOutletTransferRepository:IRepository<OutletTransfer>
    {
        List<WarehouseItem> GetWarehouseTransferItemsForUpdate(int OutletTransferId, int warehouseId);
        List<OutletItem> GetOutletTransferItemsForUpdate(int OutletTransferId, int outletId);
        List<OutletTransfer> GetAll(int? BranchId = null, int? warehouseId = null, int? outletId = null, DateTime? fromDate = null, DateTime? toDate = null);
    }
}
