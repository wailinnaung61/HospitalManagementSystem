using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Interfaces;
using MSIS_HMS.Infrastructure.Repositories.Base;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class LabResultRepository : Repository<LabResult>, ILabResultRepository
    {
        public LabResultRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }
        public override List<LabResult> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabResults");
            var labResults = ds.Tables[0].ToList<LabResult>();
            return labResults;
        }
        public override List<LabResult> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabResults", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var labResults = ds.Tables[0].ToList<LabResult>();
            return labResults;
        }
        public List<LabResult> GetAll(int? BranchId = null, int? LabResultId = null, int? PatientId = null, string ResultNo = null, int? TechnicianId = null, int? ConsultantId = null, bool? IsCompleted = null, DateTime? Date = null, DateTime? StartDate = null, DateTime? EndDate = null, DateTime? CompletedDate = null, DateTime? StartCompletedDate = null, DateTime? EndCompletedDate = null, int? LabTestId = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabResults", new Dictionary<string, object>() {
                { "BranchId", BranchId },
                { "LabResultId", LabResultId },
                { "PatientId", PatientId },
                { "ResultNo", ResultNo },
                { "IsCompleted", IsCompleted },
                { "Date", Date },
                { "StartDate", StartDate },
                { "EndDate", EndDate },
                { "CompletedDate", CompletedDate },
                { "StartCompletedDate", StartCompletedDate },
                { "EndCompletedDate", EndCompletedDate },
                { "LabTestId", LabTestId },
            });
            var labResults = ds.Tables[0].ToList<LabResult>();
            return labResults;
        }
        public override LabResult Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabResults", new Dictionary<string, object>() { { "LabResultId", Id } });
            var labResults = ds.Tables[0].ToList<LabResult>();
            var labResult = labResults.Count > 0 ? labResults[0] : null;
            if (labResult != null)
            {
                labResult.LabResultDetails = GetLabResultDetails(Id);
                labResult.LabResultFiles = GetLabResultFiles(Id);
            }

            return labResult;
        }
        public List<LabResultDetail> GetLabResultDetails(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabResultDetails", new Dictionary<string, object>() { { "LabResultId", Id } });
            var labResultTests = ds.Tables[0].ToList<LabResultDetail>();
            return labResultTests;
        }
        public List<LabResultFile> GetLabResultFiles(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetLabResultFiles", new Dictionary<string, object>() { { "LabResultId", Id } });
            var labResultTests = ds.Tables[0].ToList<LabResultFile>();
            return labResultTests;
        }
        public override async Task<LabResult> UpdateAsync(LabResult labResult)
        {
            if (labResult != null)
            {
                if(labResult.LabResultDetails!=null)
                {
                    labResult.LabResultDetails.ToList().ForEach(x => x.LabResultId = labResult.Id);
                    var existingLabResult = _context.LabResults
                        .Where(p => p.Id == labResult.Id)
                        .Include(x => x.LabResultDetails)
                        .SingleOrDefault();

                    if (existingLabResult != null)
                    {
                        // Update parent
                        labResult.CreatedAt = existingLabResult.CreatedAt;
                        labResult.CreatedBy = existingLabResult.CreatedBy;
                        if (labResult.LabTestId != existingLabResult.LabTestId)
                        {
                            labResult.UnitPrice = _context.LabTests.SingleOrDefault(x => x.Id == labResult.LabTestId).UnitPrice;
                        }
                        if (labResult.TechnicianId != null)
                        {
                            if (labResult.TechnicianId != existingLabResult.TechnicianId || labResult.LabTestId != existingLabResult.LabTestId)
                            {
                                var technicianFee = _context.LabPerson_LabTests.SingleOrDefault(x => x.LabPersonId == labResult.TechnicianId && x.LabTestId == labResult.LabTestId);
                                labResult.TechnicianFee = technicianFee.Fee;
                                labResult.TechnicianFeeType = technicianFee.FeeType;
                            }
                        }
                        else
                        {
                            labResult.TechnicianFee = default(decimal);
                            labResult.TechnicianFeeType = default(Core.Enums.FeeTypeEnum);
                        }
                        if (labResult.ConsultantId != null)
                        {
                            if (labResult.ConsultantId != existingLabResult.ConsultantId || labResult.LabTestId != existingLabResult.LabTestId)
                            {
                                var consultantFee = _context.LabPerson_LabTests.SingleOrDefault(x => x.LabPersonId == labResult.ConsultantId && x.LabTestId == labResult.LabTestId);
                                labResult.ConsultantFee = consultantFee.Fee;
                                labResult.ConsultantFeeType = consultantFee.FeeType;
                            }
                        }
                        else
                        {
                            labResult.ConsultantFee = default(decimal);
                            labResult.ConsultantFeeType = default(Core.Enums.FeeTypeEnum);
                        }
                        _context.Entry(existingLabResult).CurrentValues.SetValues(labResult);

                        // Delete children
                        _context.LabResultDetails.RemoveRange(existingLabResult.LabResultDetails);

                        // Insert children
                        existingLabResult.LabResultDetails = labResult.LabResultDetails;

                        await _context.SaveChangesAsync();
                        return labResult;
                    }
                }
                else if(labResult.LabResultFiles!=null)
                {
                    var existingLabResult = _context.LabResults
                        .Where(p => p.Id == labResult.Id)
                        .Include(x => x.LabResultFiles)
                        .SingleOrDefault();
                    if(existingLabResult!=null)
                    {
                        labResult.CreatedAt = existingLabResult.CreatedAt;
                        labResult.CreatedBy = existingLabResult.CreatedBy;
                        if (labResult.LabTestId != existingLabResult.LabTestId)
                        {
                            labResult.UnitPrice = _context.LabTests.SingleOrDefault(x => x.Id == labResult.LabTestId).UnitPrice;
                        }
                        _context.Entry(existingLabResult).CurrentValues.SetValues(labResult);

                        // Delete children
                        _context.LabResultFiles.RemoveRange(existingLabResult.LabResultFiles);

                        // Insert children
                        existingLabResult.LabResultFiles = labResult.LabResultFiles;

                        await _context.SaveChangesAsync();
                        return labResult;
                    }
                }
                
            }
            return null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            try
            {
                var labResult = await _context.LabResults.FindAsync(Id);
                if (labResult == null)
                {
                    return false;
                }
                labResult.IsDelete = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}