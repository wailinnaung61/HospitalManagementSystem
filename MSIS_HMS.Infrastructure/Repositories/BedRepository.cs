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
    public class BedRepository : Repository<Bed>, IBedRepository
    {
        public BedRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }       

        public List<Bed> GetAll(int? BedId = null, int? BedTypeId = null, string BedStatus = null, int? RoomId = null, string BedNo = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetBeds", new Dictionary<string, object>() {
                {"BedId",BedId},
                {"BedTypeId",BedTypeId },
                {"RoomId",RoomId},
                {"BedNo",BedNo },
                {"BedStatus",BedStatus}
            });
            var bed = ds.Tables[0].ToList<Bed>();
            return bed;
        }
        public override Bed Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetBeds", new Dictionary<string, object>() { { "BedId", Id } });
            var bed = ds.Tables[0].ToList<Bed>();
            return bed.Count > 0 ? bed[0] : null;
        }

        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var ward = await _context.Beds.FindAsync(Id);
                    if (ward == null)
                    {
                        return false;
                    }
                    ward.IsDelete = true;
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