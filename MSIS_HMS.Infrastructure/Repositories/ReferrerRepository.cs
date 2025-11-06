using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Entities.DTOs;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Interfaces;
using MSIS_HMS.Infrastructure.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class ReferrerRepository:Repository<Referrer>, IReferrerRepository
    {
        public ReferrerRepository(ApplicationDbContext context, IConfigService config) : base(context, config)
        {
        }
        public override List<Referrer> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetReferrers");
            var referrer = ds.Tables[0].ToList<Referrer>();
            return referrer;
        }
        

        public override List<Referrer> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetReferrers", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var referrer = ds.Tables[0].ToList<Referrer>();
            return referrer;
        }
        public override Referrer Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetReferrers", new Dictionary<string, object>() { { "ReferrerId", Id } });
            var referrers = ds.Tables[0].ToList<Referrer>();
            return referrers.Count > 0 ? referrers[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var referrer = await _context.Referrers.FindAsync(Id);
                    if (referrer == null)
                    {
                        return false;
                    }
                    referrer.IsDelete = true;
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

        public List<ReferrerReportDTO> GetReferrerReport(int? Id,DateTime? startDate,DateTime? endDate)
        {
            List<ReferrerReportDTO> referrerReportDTOs = new List<ReferrerReportDTO>();
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetReferrerFeeReport", new Dictionary<string, object>() { { "ReferrerId", Id }, { "StartDate", startDate }, { "EndDate", endDate } });
            var referrer = ds.Tables[0].ToList<ReferrerReportDTO>();
            int i = 0;
            foreach(var item in referrer)
            {
               
                if(i!=0)
                {
                    if (referrerReportDTOs[i-1].Id == item.Id)
                    {
                        if(item.LabFee!=0)
                        {
                            referrerReportDTOs[i - 1].LabFee = item.LabFee;
                        }
                        if (item.OTFee != 0)
                        {
                            referrerReportDTOs[i - 1].OTFee = item.OTFee;
                        }
                        if (item.OPDFee != 0)
                        {
                            referrerReportDTOs[i - 1].OPDFee = item.OPDFee;
                        }
                    }
                    else
                    {
                        referrerReportDTOs.Add(item);
                        i++;
                    }
                }
                else
                {
                    referrerReportDTOs.Add(item);
                    i++;
                }
                
                
            }
            return referrerReportDTOs;
        }
    }
}
