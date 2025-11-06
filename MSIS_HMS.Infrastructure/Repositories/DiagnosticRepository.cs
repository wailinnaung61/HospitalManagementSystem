using MSIS_HMS.Core.Entities;
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
    public class DiagnosticRepository : Repository<Diagnostic>, IDiagnosticRepository
    {
        public DiagnosticRepository(ApplicationDbContext context, IConfigService config) : base(context, config)
        {

        }
        public List<Diagnostic> GetAll(string DiagnosticName = null, int? DoctorId = null, int? SpecialityId = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetDiagnostic", new Dictionary<string, object>{
                {"Name",DiagnosticName },
                {"SpecialityId",SpecialityId },
                {"DoctorId", DoctorId }
            });
            var diagnostics = ds.Tables[0].ToList<Diagnostic>();
            return diagnostics;
        }
        public override Diagnostic Get(int id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetDiagnostic", new Dictionary<string, object>{
                {"Id",id }
            });
            var diagnostic = ds.Tables[0].ToList<Diagnostic>();
            return diagnostic.Count > 0 ? diagnostic[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var outlet = await _context.Diagnostics.FindAsync(Id);
                    if (outlet == null)
                    {
                        return false;
                    }
                    outlet.IsDelete = true;
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
