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
using MSIS_HMS.Core.Entities.DTOs;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class LabTestRepository : Repository<LabTest>, ILabTestRepository
    {
        public LabTestRepository(ApplicationDbContext context, IConfigService config) : base(context, config)
        {
        }
        public override List<LabTest> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabTests");
            var labTests = ds.Tables[0].ToList<LabTest>();
            return labTests;
        }
        public override List<LabTest> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabTests", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var labTests = ds.Tables[0].ToList<LabTest>();
            return labTests;
        }
        public List<LabTest> GetAll(int? BranchId = null, int? LabTestId = null, string Name = null, string Code = null, LabCategoryEnum? Category = null, bool? IsLabReport = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabTests", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "LabTestId", LabTestId },
                { "Name", Name },
                { "Code", Code },
                { "Category", Category },
                { "IsLabReport", IsLabReport },
            });
            var labTests = ds.Tables[0].ToList<LabTest>();
            return labTests;
        }
        public List<LabTest> GetLabTestByOrderId(int? labOrderId=null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabTestByOrderId", new Dictionary<string, object>() {
                { "LabOrderId", labOrderId }
               
            });
            var labTests = ds.Tables[0].ToList<LabTest>();
            return labTests;
        }
        public List<LabTestDTO> GetLabTestByLabOrderId(int labOrderId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabTestByLabOrderId", new Dictionary<string, object>() {
                { "laborderId", labOrderId }               
            });
            var labTestDTOs = ds.Tables[0].ToList<LabTestDTO>();
            return labTestDTOs;
        }
        public override LabTest Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabTests", new Dictionary<string, object>() { { "LabTestId", Id } });
            var labTests = ds.Tables[0].ToList<LabTest>();
            labTests.ForEach(x => x.LabTestDetails = GetLabTestDetails(x.Id));
            return labTests.Count > 0 ? labTests[0] : null;
        }
        public List<LabTestDetail> GetLabTestDetails(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabTestDetails", new Dictionary<string, object>() { { "LabTestId", Id } });
            var labTestDetails = ds.Tables[0].ToList<LabTestDetail>();
            return labTestDetails;
        }
        public override async Task<LabTest> UpdateAsync(LabTest labTest)
        {
            if (labTest != null)
            {
                labTest.LabTestDetails.ToList().ForEach(x => x.LabTestId = labTest.Id);
                var existingLabTest = _context.LabTests
                    .Where(p => p.Id == labTest.Id)
                    .Include(p => p.LabTestDetails)
                    .SingleOrDefault();

                if (existingLabTest != null)
                {
                    // Update parent
                    _context.Entry(existingLabTest).CurrentValues.SetValues(labTest);

                    // Delete children
                    _context.LabTestDetails.RemoveRange(existingLabTest.LabTestDetails);

                    // Insert children
                    existingLabTest.LabTestDetails = labTest.LabTestDetails;

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
                    var labTest = await _context.LabTests.FindAsync(Id);
                    if (labTest == null)
                    {
                        return false;
                    }
                    labTest.IsDelete = true;
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