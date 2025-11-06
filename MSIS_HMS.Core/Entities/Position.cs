using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using MSIS_HMS.Core.Entities.Base;
using MSIS_HMS.Core.Repositories;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Position")]
    public class Position : Entity
    {
        public Position()
        {
        }
        [Required]
        public string  Name { get; set; }
        public string  Code { get; set; }
    }
}