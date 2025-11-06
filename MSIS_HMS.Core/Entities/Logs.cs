using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MSIS_HMS.Core.Entities.Base;

namespace MSIS_HMS.Core.Entities
{
    [Table("Logs")]
    public class Logs
    {
        public Logs()
        {

        }

        public int Id { get; set; }
        [Required]
        public string MachineName { get; set; }
        [Required]
        public DateTime Logged { get; set; }
        [Required]
        public string Level { get; set; }
        [Required]
        public string Message { get; set; }
        public string Logger { get; set; }
        public string Properties { get; set; }
        public string Callsite { get; set; }
        public string Exception { get; set; }
        public string UserId { get; set; }
    }
}
