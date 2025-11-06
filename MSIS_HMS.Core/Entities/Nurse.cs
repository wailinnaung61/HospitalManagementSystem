using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using MSIS_HMS.Core.Entities.Attributes;
using MSIS_HMS.Core.Entities.Base;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;


namespace MSIS_HMS.Core.Entities
{
    [Table("Nurse")]
    public class Nurse : BranchEntity
    {
        public Nurse()
        {

        }

        [Required]
        public string Name { get; set; }
        [Required]
        public string NRC { get; set; }
        
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode=true)]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }
        
        [Required]
        [Phone]
        public string Phone { get; set; }
        public int Age { get; set; }
        [Required]
        public string Gender { get; set; }
        public string ProfileImage { get; set; }
        public string Brief { get; set; }
        [Required]
        public string Code { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [SkipProperty]
        public Department Department { get; set; }

    }
}
