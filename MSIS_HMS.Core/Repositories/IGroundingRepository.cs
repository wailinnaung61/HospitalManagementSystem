using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Repositories
{
    public interface IGroundingRepository:IRepository<Grounding>
    {
        List<Grounding> GetAll();
        List<Grounding> GetAll(int? BranchId = null, int? warehouseId = null, DateTime? fromDate = null, DateTime? toDate = null);
    }
}
