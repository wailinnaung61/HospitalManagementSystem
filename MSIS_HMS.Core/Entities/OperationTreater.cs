using MSIS_HMS.Core.Entities.Base;
using MSIS_HMS.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;
using static MSIS_HMS.Core.Enums.GenderEnum;

namespace MSIS_HMS.Core.Entities
{
    [Table("OperationTreater")]
    public class OperationTreater : BranchEntity
    {
        public OperationTreater()
        {

        }
        public DateTime OperationDate { get; set; }
        public int PatientId { get; set; }
        public int OutletId { get; set; }
        public int ChiefSurgeonDoctorId { get; set; }
        public decimal ChiefSurgeonFee { get; set; }
        public int AneasthetistDoctorId { get; set; }
        public decimal AneasthetistFee { get; set; }
        public int OperationRoomId { get; set; }
        public int OpeartionTypeId { get; set; }
        public string PaidBy { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public decimal Balance { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidDate { get; set; }
        public string Status { get; set; }
        public int? ReferrerId { get; set; }
        public decimal? ReferrerFee { get; set; }
        public OperationTypeEnum Type { get; set; }
        public string AneasthetistType { get; set; }
        public TimeSpan? OTStartTime { get; set; }
        public TimeSpan? OTEndTime { get; set; }
        public string PreOPDiagnosis { get; set; }
        public string PostOPDiagnosis { get; set; }
        public string OperationProcedure { get; set; }
        public bool SentToPathology { get; set; }
        public bool ByDoctor { get; set; }
        public string Findings { get; set; }
        public string PostOpTreatment { get; set; }
        

        [NotMapped]
        public string PatientName { get; set; }
        [NotMapped]
        public int PatientAge { get; set; }
        [NotMapped]
        public Gender PatientGender { get; set; }
        [NotMapped]
        public string DoctorName { get; set; }
        [NotMapped]
        public string AneasthetistName { get; set; }
        [NotMapped]
        public string BranchName { get; set; }
        [NotMapped]
        public string OperationTypeName { get; set; }
        [NotMapped]
        public string RoomNo { get; set; }
        [NotMapped]
        public string ReferrerName { get; set; }
        [NotMapped]
        [SkipProperty]
        public string OperationType { get; set; }
        [SkipProperty]
        public Outlet Outlet { get; set; }
        [SkipProperty]
        public Patient Patient { get; set; }
        [SkipProperty]
        public Doctor Doctor { get; set; }
        [SkipProperty]
        public Referrer Referrer { get; set; }
        [SkipProperty]
        public ICollection<OperationItem> OperationItems { get; set; }
        [SkipProperty]
        public ICollection<OperationService> OperationServices { get; set; }
        [SkipProperty]
        public ICollection<OT_Doctor> OT_Doctors { get; set; }
        [SkipProperty]
        public ICollection<OT_Staff> OT_Staffs { get; set; }
        [SkipProperty]
        public ICollection<OT_Anaesthetist> OT_Anaesthetists { get; set; }

    }
}
