using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Repositories.Base;

namespace MSIS_HMS.Core.Repositories
{
    public interface IWarehouseRepository : IRepository<Warehouse>
    {
        Task UpdateStockAsync(List<WarehouseItem> warehouseItems, int? action = null);
        Task ReplaceStockAsync(List<WarehouseItem> warehouseItems);
        List<WarehouseItemDTO> GetWarehouseItemDTOs(int? BranchId, int? WarehouseId, int? ItemId, int? BatchId, DateTime? ExpiryDate, DateTime? StartExpiryDate, DateTime? EndExpiryDate);
        List<Item> GetItemsFromWarehouse(int BranchId, int WarehouseId);
        List<Batch> GetBatchesOfWarehouseItem(int BranchId, int WarehouseId, int ItemId);
        List<WarehouseItemLocationDTO> GetWarehouseItemLocationsDTO(int? BranchId, int? WarehouseId, int? ItemId, int? LocationId);
        List<WarehouseItemDTO> GetWarehouseItemReport(int? BranchId, int? WarehouseId, int? ItemId, int? BatchId, DateTime? ExpiryDate, DateTime? StartExpiryDate, DateTime? EndExpiryDate);
    }
}