using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("AppointmentType")]
    public class AppointmentType:BranchEntity
    {
        public AppointmentType()
        {
        }

        [Required]
        public string Type { get; set; }
        public string Remark { get; set; }

        [SkipProperty]
        public ICollection<Appointment> Appointments { get; set; }
    }
}
