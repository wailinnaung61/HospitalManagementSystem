using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MSIS_HMS.Core.Repositories.Base;
using MSIS_HMS.Core.Entities;

namespace MSIS_HMS.Core.Repositories
{
    public interface IBranchRepository : IRepository<Branch>
    {
        List<Branch> GetNonMainBranch(int MainBranchId);
    }
}
