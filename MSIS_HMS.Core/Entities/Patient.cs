using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using MSIS_HMS.Core.Entities.Base;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;
using static MSIS_HMS.Core.Enums.GenderEnum;

namespace MSIS_HMS.Core.Entities
{
    [Table("Patient")]
    public class Patient : BranchEntity
    {
        public Patient()
        {
        }

        public DateTime RegDate { get; set; }
        [Required]
        public string RegNo { get; set; }
        [Required]
        public string Name { get; set; }
        public string NRC { get; set; }
        [Required]
        public string Guardian { get; set; }
        public string Image { get; set; }
        public Gender Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }
        public int? AgeYear { get; set; }
        public int? AgeMonth { get; set; }
        public int? AgeDay { get; set; }
        public string Address { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public int TownshipId { get; set; }
        public string Phone { get; set; }
        public string Occupation { get; set; }
        public string Status { get; set; }
        public string BloodType { get; set; }
        public int? ReferrerId { get; set; }
        public string ReferredBy { get; set; }
        public DateTime? ReferredDate { get; set; }
        public string Religion { get; set; }
        public string Allergies { get; set; }
        public bool IsActive { get; set; }
        public string QRCode {get;set;}
        public string BarCode { get; set; }
        public string Code { get; set; }

        [NotMapped]
        [SkipProperty]
        public string ImageContent { get; set; }
        [NotMapped]
        [SkipProperty]
        public IFormFile ImageFile { get; set; }
        [SkipProperty]
        public ICollection<IPDRecord> IPDRecords { get; set; }
        [SkipProperty]
        public Referrer Referrer { get; set; }
        [SkipProperty]
        public ICollection<Order> Orders { get; set; }
    }
}
