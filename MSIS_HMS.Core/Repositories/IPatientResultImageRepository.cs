using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Repositories
{
    public interface IPatientResultImageRepository : IRepository<PatientResultImage>
    {
        List<PatientResultImage> GetAll(int? BranchId = null, int? PatientId = null, DateTime? StartDate = null, DateTime? EndDate = null);
        PatientImageDTO Get(int Id);
    }
}
