using System.Collections.Generic;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Enums;
using MSIS_HMS.Core.Repositories.Base;

namespace MSIS_HMS.Core.Repositories
{
    public interface ILabPersonRepository : IRepository<LabPerson>
    {
        List<LabPerson> GetAll(int? BranchId = null, int? LabPersonId = null, string Name = null, string Code = null, LabPersonTypeEnum? Type = null, int? DepartmentId = null, int? DoctorId = null);
        LabPerson GetInfo(int Id);
        List<LabPerson_LabTest> GetLabPerson_LabTests(int? LabPersonId = null, int? LabTestId = null);
    }
}