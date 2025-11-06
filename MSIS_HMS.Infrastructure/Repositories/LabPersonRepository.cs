using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Enums;
using MSIS_HMS.Core.Repositories.Base;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Interfaces;
using MSIS_HMS.Infrastructure.Repositories.Base;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MSIS_HMS.Core.Repositories;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class LabPersonRepository : Repository<LabPerson>, ILabPersonRepository
    {
        public LabPersonRepository(ApplicationDbContext context, IConfigService config) : base(context, config)
        {
        }
        public override List<LabPerson> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabPersons");
            var labPersons = ds.Tables[0].ToList<LabPerson>();
            return labPersons;
        }
        public override List<LabPerson> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabPersons", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var labPersons = ds.Tables[0].ToList<LabPerson>();
            return labPersons;
        }
        public List<LabPerson> GetAll(int? BranchId = null, int? LabPersonId = null, string Name = null, string Code = null, LabPersonTypeEnum? Type = null, int? DepartmentId = null, int? DoctorId = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabPersons", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "LabPersonId", LabPersonId },
                { "Name", Name },
                { "Code", Code },
                { "Type", Type },
                { "DepartmentId", DepartmentId },
                { "DoctorId", DoctorId },
            });
            var labPersons = ds.Tables[0].ToList<LabPerson>();
            return labPersons;
        }
        public override LabPerson Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabPersons", new Dictionary<string, object>() { { "LabPersonId", Id } });
            var labPersons = ds.Tables[0].ToList<LabPerson>();
            labPersons.ForEach(x => x.LabPerson_LabTests = GetLabPerson_LabTests(x.Id));
            return labPersons.Count > 0 ? labPersons[0] : null;
        }
        public LabPerson GetInfo(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabPersons", new Dictionary<string, object>() { { "LabPersonId", Id } });
            var labPersons = ds.Tables[0].ToList<LabPerson>();
            return labPersons.Count > 0 ? labPersons[0] : null;
        }
        public List<LabPerson_LabTest> GetLabPerson_LabTests(int? LabPersonId = null, int? LabTestId = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabPerson_LabTests", new Dictionary<string, object>() { { "LabPersonId", LabPersonId }, { "LabTestId", LabTestId } });
            var labPerson_LabTests = ds.Tables[0].ToList<LabPerson_LabTest>();
            return labPerson_LabTests;
        }
        public override async Task<LabPerson> UpdateAsync(LabPerson labTest)
        {
            if (labTest != null)
            {
                labTest.LabPerson_LabTests?.ToList().ForEach(x => x.LabPersonId = labTest.Id);
                var existingLabPerson = _context.LabPersons
                    .Where(p => p.Id == labTest.Id)
                    .Include(p => p.LabPerson_LabTests)
                    .SingleOrDefault();

                if (existingLabPerson != null)
                {
                    // Update parent
                    _context.Entry(existingLabPerson).CurrentValues.SetValues(labTest);

                    // Delete children
                    _context.LabPerson_LabTests.RemoveRange(existingLabPerson.LabPerson_LabTests);

                    // Insert children
                    existingLabPerson.LabPerson_LabTests = labTest.LabPerson_LabTests;

                    await _context.SaveChangesAsync();
                    return labTest;
                }
            }
            return null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var labPerson = await _context.LabPersons.FindAsync(Id);
                    if (labPerson == null)
                    {
                        return false;
                    }
                    labPerson.IsDelete = true;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (DbException e)
                {
                    Console.WriteLine(e.Message);
                    await transaction.RollbackAsync();
                }
            }
            return false;
        }

    }
}