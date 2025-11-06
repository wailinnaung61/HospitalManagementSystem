using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Module")]
    public class Module: Entity
    {
        public Module()
        {
        }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [Required]
        public string DisplayName_mm { get; set; }
        public string Description { get; set; }
        public bool IsMandatory { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }
        public int? ModuleOrder { get; set; }

        [SkipProperty]
        public ICollection<Menu> Menus { get; set; }
    }
}
