using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using MSIS_HMS.Core.Entities.Attributes;
using MSIS_HMS.Core.Entities.Base;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;
using static MSIS_HMS.Core.Enums.GenderEnum;

namespace MSIS_HMS.Core.Entities
{
    [Table("Doctor")]
    public class Doctor : BranchEntity
    {
        public Doctor()
        {
        }
        [Required]
        public string Name { get; set; }

        [Required]
        public string NRC { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }


        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        public int SpecialityId { get; set; }
        [NotMapped]
        public string SpecialityName { get; set; }
        [SkipProperty]
        public Speciality Speciality { get; set; }

        public int? Age { get; set; }

        [Required]
        public Gender Gender { get; set; }

        public string ProfileImage { get; set; }
        public string Image { get; set; }
        public string Brief { get; set; }

        [Required]
        public string SamaNumber { get; set; }

        public int? CFFee { get; set; }

        public decimal? RoundFee { get; set; }
        public decimal CFFeeForHospital { get; set; }
        public decimal RoundFeeForHospital { get; set; }
        public int EstimatewaitingTime { get; set; }
        public decimal? ReadingFee { get; set; }

        [Required]
        public string Code { get; set; }

        [NotMapped]
        [SkipProperty]
        [Required]
        public string ImageContent { get; set; }
        [NotMapped]
        [SkipProperty]
        public IFormFile ImageFile { get; set; }
        [SkipProperty]
        public ICollection<Patient> Patients { get; set; }
        [SkipProperty]
        public ICollection<Order> Orders { get; set; }
        [SkipProperty]
        public ICollection<Schedule> Schedules { get; set; }
        [SkipProperty]
        public ICollection<Symptom> Symptoms { get; set; }
        [SkipProperty]
        public ICollection<Diagnostic> Diagnostics { get; set; }
        [SkipProperty]
        public ICollection<IPDRound> IPDRounds { get; set; }
        [SkipProperty]
        public ICollection<LabPerson> LabPersons { get; set; }
    }
}