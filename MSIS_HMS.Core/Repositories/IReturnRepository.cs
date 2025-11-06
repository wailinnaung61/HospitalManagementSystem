using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Repositories
{
    public interface IReturnRepository:IRepository<Return>
    {
        List<WarehouseItem> GetWarehouseTransferItemsForUpdate(int ReturnId, int warehouseId);
        List<OutletItem> GetOutletTransferItemsForUpdate(int ReturnId, int outletId);
        List<Return> GetAll(int? BranchId = null, int? warehouseId = null, int? outletId = null, DateTime? fromDate = null, DateTime? toDate = null);
    }
}
