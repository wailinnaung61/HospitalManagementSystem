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
    public class FoodRepository : Repository<Food>, IFoodRepository
    {
        public FoodRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {

        }
        public override List<Food> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetFoods");
            var food = ds.Tables[0].ToList<Food>();
            return food;
        }

        public List<Food> GetAll(int? FoodId = null, int? FoodCategoryId = null, string Name = null, decimal? UnitPrice = null, string Code = null, string Description = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetFoods", new Dictionary<string, object>() {
               {"FoodId",FoodId},
                {"FoodCategoryId",FoodCategoryId },
                {"Name",Name },
                {"UnitPrice",UnitPrice },
                {"Code",Code},
                {"Description",Description }
            });
            var food = ds.Tables[0].ToList<Food>();
            return food;
        }
        public override Food Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetFoods", new Dictionary<string, object>() { { "FoodId", Id } });
            var food = ds.Tables[0].ToList<Food>();
            return food.Count > 0 ? food[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var food = await _context.Foods.FindAsync(Id);
                    if (food == null)
                    {
                        return false;
                    }
                    food.IsDelete = true;
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