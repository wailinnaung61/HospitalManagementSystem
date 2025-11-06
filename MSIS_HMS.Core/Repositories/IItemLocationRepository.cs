using System.Collections.Generic;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;

namespace MSIS_HMS.Core.Repositories
{
    public interface IItemLocationRepository : IRepository<ItemLocation>
    {
        public List<ItemLocation> GetAll(int? BranchId = null, int? ItemId = null, int? LocationId = null, int? ItemLocationId = null, int? BatchId = null, int? WarehouseId = null);
    }
}