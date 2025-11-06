using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("ItemType")]
    public class ItemType : BranchEntity
    {
        public ItemType()
        {
        }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        [SkipProperty]
        public ICollection<Item> Items { get; set; }
    }
}
