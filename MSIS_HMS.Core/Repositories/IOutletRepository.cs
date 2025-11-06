using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSIS_HMS.Core.Repositories
{
    public interface IOutletRepository : IRepository<Outlet>
    {
        List<Outlet> GetAll(int? BranchId = null, string OutletName = null, string OutletCode = null, int? WarehouseId = null, int? OutletId = null);
        Task UpdateStockAsync(List<OutletItem> outletItems, int? action = null);
        List<Item> GetItemsFromOutlet(int BranchId, int WarehouseId);
        List<OutletStockItemDTO> GetOutetStocks(int? BranchId, int? WarehouseId, int? OutletId, int? ItemId);
    }
}
