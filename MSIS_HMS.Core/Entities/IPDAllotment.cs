using MSIS_HMS.Core.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("IPDAllotment")]
    public class IPDAllotment : Entity
    {
        public DateTime Date { get; set; }
        public int? FromRoomId { get; set; }
        public int ToRoomId { get; set; }
        public int? FromBedId { get; set; }
        public int? ToBedId { get; set; }
        public int IPDRecordId  { get; set; }
        public string Reason { get; set; }
        public  int? IPDOrderId { get; set; }
        public decimal UnitPrice { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
   

        [SkipProperty]
        [NotMapped]
        public TimeSpan? NewCheckInTime { get; set; }
        [SkipProperty]
        [NotMapped]
        public TimeSpan? NewCheckOutTime { get; set; }

        [NotMapped]
        public string FromRoomNo { get; set; }
        [NotMapped]
        public string ToRoomNo { get; set; }
        [NotMapped]
        public string FromBedNo { get; set; }
        [NotMapped]
        public string ToBedNo { get; set; }
       
        
        [SkipProperty]
        public Room ToRoom { get; set; }               
        [SkipProperty]
        public IPDRecord IPDRecord { get; set; }

    }
}
