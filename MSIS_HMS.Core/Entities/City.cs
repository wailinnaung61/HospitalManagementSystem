using MSIS_HMS.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("City")]
    public class City : Entity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int StateId { get; set; }
        [SkipProperty]
        public State State { get; set; }
        [SkipProperty]
        public ICollection<Township> Townships { get; set; }
       
    }
}
