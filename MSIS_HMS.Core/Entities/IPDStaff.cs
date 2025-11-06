using MSIS_HMS.Core.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("IPDStaff")]
    public class IPDStaff:Entity
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Fee { get; set; }
        public bool IsFOC { get; set; }
        public int SortOrder { get; set; }
        public int StaffId { get; set; }
        public int IPDRecordId { get; set; }

        [NotMapped]
        public string StaffName { get; set; }
        [SkipProperty]
        public IPDRecord IPDRecord { get; set; }
        [SkipProperty]
        public Staff Staff { get; set; }
    }
}
