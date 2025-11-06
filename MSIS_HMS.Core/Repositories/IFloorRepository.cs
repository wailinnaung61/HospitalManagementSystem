using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Repositories
{
    public interface IFloorRepository : IRepository<Floor>
    {
        List<Floor> GetAll(int? BranchId = null, int? FloorId=null, string Name=null);
    }
}