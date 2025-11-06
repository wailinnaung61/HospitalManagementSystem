using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;
using MSIS_HMS.Core.Entities.Base;
using System.ComponentModel;

namespace MSIS_HMS.Core.Entities
{
    [Table("Branch")]
    public class Branch : Entity
    {
        public Branch()
        {
        }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        public bool IsMainBranch { get; set; }
        public string Address { get; set; }
        [Phone]
        public string Phone { get; set; }
        public int CountryId { get; set; }
        public int TownshipId { get; set; }
        public int CityId { get; set; }
        public int StateId { get; set; }
        public string Logo { get; set; }
        public string Email { get; set; }
        

        [Required]
        [DefaultValue("P-000000")]
        public string PatientRegFormat { get; set; }
        public int PatientRegNo { get; set; }
        [Required]
        [DefaultValue("000000")]
        public string VisitNoFormat { get; set; }
        public int VisitNo { get; set; }

        [DefaultValue(false)]
        public bool AutoPaidForIPD { get; set; }
        public bool UseVoucherFormatForIPD { get; set; }
        public string VoucherFormatForIPD { get; set; }
        public int VoucherNoForIPD { get; set; }
        public TimeSpan CheckInTime { get; set; }
        public TimeSpan CheckOutTime { get; set; }
        public decimal ClinicFee { get; set; }

        [DefaultValue(false)]
        public bool AutoPaidForOrder { get; set; }
        public bool UseVoucherFormatForOrder { get; set; }
        public string VoucherFormatForOrder { get; set; }
        public int VoucherNoForOrder { get; set; }

        [DefaultValue(false)]
        public bool AutoPaidForLab { get; set; }
        public bool UseVoucherFormatForLab { get; set; }
        public string VoucherFormatForLab { get; set; }
        public int VoucherNoForLab { get; set; }

        [DefaultValue(false)]
        public bool UseVoucherFormatForLabResult { get; set; }
        public string VoucherFormatForLabResult { get; set; }
        public int VoucherNoForLabResult { get; set; }

        public bool UseBatchNoFormat { get; set; }
        public string BatchNoFormat { get; set; }
        public int BatchNo { get; set; }

        public bool UseVoucherFormatForPurchase { get; set; }
        public string VoucherFormatForPurchase { get; set; }
        public int VoucherNoForPurchase { get; set; }

        public bool UseVoucherFormatForDeliverOrder { get; set; }
        public string VoucherFormatForDeliverOrder { get; set; }
        public int VoucherNoForDeliverOrder { get; set; }

        public bool UseVoucherFormatForImaging { get; set; }
        public string VoucherFormatForImaging { get; set; }
        public int VoucherNoForImaging { get; set; }

        [NotMapped]
        public string StateName { get; set; }
        [NotMapped]
        public string CityName { get; set; }
        [NotMapped]
        public string TownshipName { get; set; }


        [SkipProperty]
        public ICollection<Department> Departments { get; set; }
        [SkipProperty]
        public ICollection<Warehouse> Warehouses { get; set; }
        [SkipProperty]
        public ICollection<Outlet> Outlets { get; set; }
        [SkipProperty]
        public ICollection<Location> Locations { get; set; }
        [SkipProperty]
        public ICollection<ItemLocation> ItemLocations { get; set; }
        [SkipProperty]
        public ICollection<Patient> Patients { get; set; }
        [SkipProperty]
        public ICollection<Order> Orders { get; set; }
    }
}
