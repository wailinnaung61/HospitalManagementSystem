using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Repositories
{
    public interface ILocationRepository : IRepository<Location>
    {
        List<Location> GetAll(int? BranchId = null, string LocationName = null, string LocationCode = null, int? WarehouseId = null,int? LocationId=null);
    }
}
