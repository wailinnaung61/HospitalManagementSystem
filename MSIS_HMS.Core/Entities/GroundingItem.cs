using MSIS_HMS.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("GroundingItem")]
   public class GroundingItem
    {
        public GroundingItem()
        {

        }
        public int Id { get; set; }
        public int GroundingId { get; set; }
        public int ItemId { get; set; }
        public int PreviouseQty { get; set; }
        public int ChangedQty { get; set; }
        public int BatchId { get; set; }
        public int UnitId { get; set; }
        [NotMapped]
        [SkipProperty]
        public string ItemName { get; set; }
        [SkipProperty]
        public Grounding Grounding { get; set; }
        [NotMapped]
        [SkipProperty]
        public string BatchName { get; set; }
        [NotMapped]
        [SkipProperty]
        public string UnitName { get; set; }
        [SkipProperty]
        public Item Item { get; set; }
        [SkipProperty]
        public Batch Batch { get; set; }
        [SkipProperty]
        public Unit Unit { get; set; }

    }
}
