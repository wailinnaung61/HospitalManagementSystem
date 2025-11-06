using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSIS_HMS.Core.Repositories
{
    public interface IFoodRepository:IRepository<Food>
    {
        List<Food> GetAll(int? FoodId = null, int? FoodCategoryId = null, string Name = null, decimal? UnitPrice = null, string Code = null, string Description = null);
    }
}
