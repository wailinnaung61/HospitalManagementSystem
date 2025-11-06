using MSIS_HMS.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Staff")]
    public class Staff:BranchEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string NRC { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public string DOB { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public  int PositionId { get; set; }        
        public int? Age { get; set; }
        [Required]
        public string Gender { get; set; }
     
        public string ProfileImage { get; set; }      
        public string Brief { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        [Required]
        public string Code { get; set; }
        [NotMapped]
        public string DepartmentName { get; set; }
        [SkipProperty]
        public Department Department { get; set; }
        [SkipProperty]
        public Position Position { get; set; }
        [NotMapped]
        public string PositionName { get; set; }
        [SkipProperty]
        public ICollection<IPDStaff> IPDStaffs { get; set; } 
    }
}
