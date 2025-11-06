using System.Collections.Generic;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;

namespace MSIS_HMS.Core.Repositories
{
    public interface IDoctorRepository : IRepository<Doctor>
    {
        List<Doctor> GetAll(int? BranchId = null, int? DoctorId = null, string Name = null, string Code = null, string SamaNo = null, int? DepartmentId = null, int? DepartmentType = null, int? SpecialityId = null);
    }
}