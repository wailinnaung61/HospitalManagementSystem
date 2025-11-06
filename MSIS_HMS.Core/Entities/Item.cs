using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using MSIS_HMS.Core.Entities.Base;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Item")]
    public class Item : BranchEntity
    {
        public Item()
        {
        }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        public string Barcode { get; set; }
        public string Image { get; set; }
        public string Brand { get; set; }
        public string ChemicalName { get; set; }
        public string Composition { get; set; }
        public string Classification { get; set; }
        public string Country { get; set; }
        public string Manufacturer { get; set; }
        public string Description { get; set; }
        public int ItemTypeId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal PercentageForSale { get; set; }
        public decimal PercentageForDiscount { get; set; }
        public string Remark { get; set; }
        
        public int? ExpirationRemindDay { get; set;}

        [NotMapped]
        [SkipProperty]
        public string ImageContent { get; set; }
        [NotMapped]
        [SkipProperty]
        public IFormFile ImageFile { get; set; }
        [NotMapped]
        [SkipProperty]
        public int Qty { get; set; }  
        [SkipProperty]
        public ItemType ItemType { get; set; }
        [SkipProperty]
        public ICollection<ItemLocation> ItemLocations { get; set; }
        [SkipProperty]
        public ICollection<PackingUnit> PackingUnits { get; set; }

        [SkipProperty]
        public ICollection<Batch> Batches { get; set; }
        [SkipProperty]
        public ICollection<PurchaseItem> PurchaseItems { get; set; }
        [SkipProperty]
        public ICollection<WarehouseItem> WarehouseItems { get; set; }
        [SkipProperty]
        public ICollection<OutletItem> OutletItems { get; set; }
        [SkipProperty]
        public ICollection<WarehouseTransferItem> WarehouseTransferItems { get; set; }
        [SkipProperty]
        public ICollection<OutletTransferItem> OutletTransferItems { get; set; }
        [SkipProperty]
        public ICollection<DeliverOrderItem> DeliverOrderItems { get; set; }
        [SkipProperty]
        public ICollection<Prescription> Prescriptions { get; set; }
    }
}