using MSIS_HMS.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using MSIS_HMS.Core.Enums;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    
    [Table("FoodCategory")]
    public class FoodCategory : Entity
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [SkipProperty]
        public ICollection<Food> Foods { get; set; }

    }
}
