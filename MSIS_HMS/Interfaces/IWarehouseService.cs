using Microsoft.AspNetCore.Mvc.Rendering;
using MSIS_HMS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSIS_HMS.Interfaces
{
    public interface IWarehouseService
    {
        List<Warehouse> GetAll();
        SelectList GetSelectListItems(int? warehouseId = null);
        SelectList GetSelectListItemsByBranch(int? branchId=null, int? warehouseId = null);
    }
}
