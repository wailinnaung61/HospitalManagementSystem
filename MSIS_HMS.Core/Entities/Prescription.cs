using System;
using System.ComponentModel.DataAnnotations.Schema;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Prescription")]
    public class Prescription
    {
        public Prescription()
        {
        }

        public int Id { get; set; }
        public int MedicalRecordId { get; set; }
        public int? ItemId { get; set; }    // Drug
        public string Drug { get; set; }    // Drug's Name
        public string DirectionsForUse { get; set; }
        public string Day { get; set; }
        public string Remark { get; set; }
        public int SortOrder { get; set; }

        [SkipProperty]
        public MedicalRecord MedicalRecord { get; set; }
        [SkipProperty]
        public Item Item { get; set; }
    }
}
