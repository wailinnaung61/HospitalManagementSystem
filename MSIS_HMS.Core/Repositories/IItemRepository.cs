using System;
using System.Collections.Generic;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Repositories.Base;

namespace MSIS_HMS.Core.Repositories
{
    public interface IItemRepository : IRepository<Item>
    {
        List<Item> GetAll(int? BranchId = null, int? ItemId = null, int? ItemTypeId = null, string Name = null, string Code = null, string Barcode = null);
        List<Item> GetAllWithPackingUnits();
        List<Item> GetAllWithPackingUnits(int? BranchId);
        List<Item> GetAllWithPackingUnits(int? BranchId = null, int? ItemId = null, int? ItemTypeId = null, string Name = null, string Code = null, string Barcode = null);
        Item GetWithPackingUnit(int Id);
        List<PackingUnit> GetPackingUnits(int itemId);
        Unit GetUnit(int Id);
        List<Item> GetExpirationRemindDay(int? BranchId = null, int? ItemId = null, int? ItemTypeId = null, string Name = null, string Code = null, string Barcode = null);
        List<SaleItemDTO> GetSaleItem(int? BranchId = null, DateTime? FromDate = null, DateTime? ToDate = null, int? OutletId = null);
    }
}
