using System;
using System.Collections.Generic;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;

namespace MSIS_HMS.Core.Repositories
{
    public interface IBatchRepository : IRepository<Batch>
    {
        List<Batch> GetAll(int? BranchId, int? ItemId);
        List<Batch> GetAll(int? BranchId, int? ItemId, int? BatchId = null, string BatchName = null, string BatchCode = null, string BatchNumber = null, DateTime? ExpiryDate = null, DateTime? StartExpiryDate = null, DateTime? EndExpiryDate = null);
    }
}
