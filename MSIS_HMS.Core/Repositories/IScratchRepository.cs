using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Repositories
{
    public interface IScratchRepository : IRepository<Scratch>
    {
        List<WarehouseItem> GetScratchItemsForUpdate(int ScratchId);
        List<Scratch> GetAll(int? BranchId, int? ScratchId, int? WarehouseId, int? ItemId, DateTime? StartDate = null, DateTime? EndDate = null);
    }
}
