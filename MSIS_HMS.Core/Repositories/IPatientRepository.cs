using System;
using System.Collections.Generic;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;

namespace MSIS_HMS.Core.Repositories
{
    public interface IPatientRepository : IRepository<Patient>
    {
        List<Patient> GetAll(int? BranchId = null, int? PatientId = null, DateTime? StartRegDate = null, DateTime? EndRegDate = null, string RegNo = null, string Name = null, string NRC = null, string Guardian = null, DateTime? DateOfBirth = null, string Phone = null, string BloodType = null,string Code=null, int? ReferrerId = null);
    }
}