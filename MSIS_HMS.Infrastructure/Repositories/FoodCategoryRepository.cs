using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Interfaces;

namespace MSIS_HMS.Infrastructure.Repositories.Base
{
    public class FoodCategoryRepository :Repository<FoodCategory>, IFoodCategoryRepository
    {
        public FoodCategoryRepository(ApplicationDbContext context, IConfigService configService) : base(context, configService)
        {
        }
        public override List<FoodCategory> GetAll()
        { 
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetFoodCategories");
            var foodcategories = ds.Tables[0].ToList<FoodCategory>();
            return foodcategories;
        }
       
        public override FoodCategory Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetFoodCategories", new Dictionary<string, object>() { { "FoodCategoryId", Id } });
            var foodcategories = ds.Tables[0].ToList<FoodCategory>();
            return foodcategories.Count > 0 ? foodcategories[0] : null;
        }
        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var foodcategories = await _context.FoodCategories.FindAsync(Id);
                    if (foodcategories == null)
                    {
                        return false;
                    }
                    foodcategories.IsDelete = true;
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