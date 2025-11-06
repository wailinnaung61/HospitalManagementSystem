
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
    public class StaffRepository: Repository<Staff>, IStaffRepository
    {
        public StaffRepository(ApplicationDbContext context, IConfigService config) : base(context, config)
        {
        }
        public override List<Staff> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetStaffs");
            var staff = ds.Tables[0].ToList<Staff>();
            return staff;
        }
        public override List<Staff> GetAll(int? BranchId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetStaffs", new Dictionary<string, object>() { { "BranchId", BranchId } });
            var staff = ds.Tables[0].ToList<Staff>();
            return staff;
        }
        public List<Staff> GetAllByPosition(int? BranchId, int? PositionId = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetStaffs", new Dictionary<string, object>()
            { { "BranchId", BranchId },
            { "PositionId", PositionId }});
            var staff = ds.Tables[0].ToList<Staff>();
            return staff;
        }
        public override Staff Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetStaffs", new Dictionary<string, object>() { { "StaffId", Id } });
            var staffs = ds.Tables[0].ToList<Staff>();
            return staffs.Count > 0 ? staffs[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var staff = await _context.Staffs.FindAsync(Id);
                    if (staff == null)
                    {
                        return false;
                    }
                    staff.IsDelete = true;
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
