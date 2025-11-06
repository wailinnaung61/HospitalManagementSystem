using MSIS_HMS.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("State")]
    public class State:Entity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int CountryId { get; set; }
        [SkipProperty]
        public Country Country { get; set; }
        [SkipProperty]
        public ICollection<City> Cities { get; set; }
        
    }
}
