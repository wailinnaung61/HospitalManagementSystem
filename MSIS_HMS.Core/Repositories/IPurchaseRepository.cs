using System;
using System.Collections.Generic;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;

namespace MSIS_HMS.Core.Repositories
{
    public interface IPurchaseRepository : IRepository<Purchase>
    {
        List<Purchase> GetAll(int? BranchId, int? PurchaseId, int? WarehouseId, int? ItemId, string VoucherNo = null, string Supplier = null, DateTime? PurchaseDate = null, DateTime? StartPurchaseDate = null, DateTime? EndPurchaseDate = null);
        List<PurchaseItem> GetPurchaseItems(int Id);
        List<WarehouseItem> GetPurchaseItemsForUpdate(int PurchaseId);
    }
}