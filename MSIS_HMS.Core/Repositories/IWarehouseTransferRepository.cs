using System;
using System.Collections.Generic;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;

namespace MSIS_HMS.Core.Repositories
{
    public interface IWarehouseTransferRepository : IRepository<WarehouseTransfer>
    {
        List<WarehouseTransfer> GetAll(int? BranchId, int? WarehouseTransferId, int? FromWarehouseId, int? ToWarehouseId, int? ItemId, string Remark = null, DateTime? Date = null, DateTime? StartDate = null, DateTime? EndDate = null);
        //List<WarehouseTransferItem> GetWarehouseTransferItems(int Id);
        List<WarehouseItem> GetWarehouseTransferItemsForUpdate(int WarehouseTransferId, int WarehouseId);
    }
}