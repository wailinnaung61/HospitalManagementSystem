using MSIS_HMS.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Township")]
    public class Township:Entity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int CityId { get; set; }
        [SkipProperty]
        public City City { get; set; }
      

    }
}
