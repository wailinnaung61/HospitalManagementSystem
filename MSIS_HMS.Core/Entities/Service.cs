using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MSIS_HMS.Core.Entities.Base;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table(name:"Service")]
    public class Service : BranchEntity
    {
        [Required]
        public String Name { get; set; }
        
        public String Description { get; set; }
        
        [Required]
        public int ServiceTypeId { get; set; }
        
        public decimal ServiceFee { get; set; }
        
        public String Code { get; set; }
        [SkipProperty]
        public ServiceType ServiceType { get; set; }
        
    }
}