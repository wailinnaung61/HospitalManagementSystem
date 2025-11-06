using MSIS_HMS.Core.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("IPDFood")]
    public class IPDFood:Entity
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int IPDRecordId { get; set; }
        public int FoodId { get; set; }
        public int Qty { get; set; }
        public int UnitPrice { get; set; }
        public bool IsFOC { get; set; }
        public int SortOrder { get; set; }

        [NotMapped]
        public string FoodName { get; set; }
        [SkipProperty]
        public Food Food { get; set; }   
        [SkipProperty]
        public IPDRecord IPDRecord { get; set; }
    }
}
