using MSIS_HMS.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Floor")]
    public class Floor:BranchEntity
    {
        public string Name { get; set; }
        [SkipProperty]
        public ICollection<Ward> Wards { get; set; }
    }
}
