using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Attributes;
using MSIS_HMS.Core.Entities.Base;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;


namespace MSIS_HMS.Core.Entities
{
    [Table("Speciality")]
    public class Speciality : BranchEntity
    {
        public Speciality()
        {
                
        }
        
        [Required]
        public string Name { get; set; }
        
    }
}