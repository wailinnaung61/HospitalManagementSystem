using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using MSIS_HMS.Core.Enums;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Food")]
    public class Food : Entity
    {
        [Required]
        public string Name { get; set; }        
        public decimal UnitPrice { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int FoodCategoryId { get; set; }
        [NotMapped]
        [SkipProperty]
        public string FoodCategoryName { get; set; }
        [SkipProperty]
        public FoodCategory FoodCategory { get; set; }

    }
}
