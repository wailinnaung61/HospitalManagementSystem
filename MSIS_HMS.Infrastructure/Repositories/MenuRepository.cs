using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Interfaces;
using MSIS_HMS.Infrastructure.Repositories.Base;

namespace MSIS_HMS.Infrastructure.Repositories
{
    public class MenuRepository : Repository<Menu>, IMenuRepository
    {
        public MenuRepository(ApplicationDbContext context, IConfigService config) : base(context, config)
        {
        }

        public override List<Menu> GetAll()
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetMenus");
            var menus = ds.Tables[0].ToList<Menu>();
            return menus;
        }

        public override Menu Get(int Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetMenus", new Dictionary<string, object>() { { "MenuId", Id } });
            var menus = ds.Tables[0].ToList<Menu>();
            return menus.Count() > 0 ? menus[0] : null;
        }

        public List<Menu> GetParentList(bool hasChildList = false)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetMenus", new Dictionary<string, object>() { { "IsParent", true } });
            var menus = ds.Tables[0].ToList<Menu>();
            if (hasChildList)
            {
                menus.ForEach(x => x.ChildMenus = GetChildList(x.Id));
            }
            return menus;
        }

        public List<Menu> GetChildListByBranchId(int ParentId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetMenus", new Dictionary<string, object>() { { "IsParent", false }, { "ParentId", ParentId },{ "IsBranchId", true } });
            var menus = ds.Tables[0].ToList<Menu>().Where(x => x.GroupId == null).ToList();
            menus.ForEach(x =>
            {
                if (x.IsGroup)
                {    
                    x.ChildMenus = GetGroupItemList((int)x.Id);
                }
            });
            return menus;
        }
        
        public List<Menu> GetChildList(int ParentId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetMenus", new Dictionary<string, object>() { { "IsParent", false }, { "ParentId", ParentId } });
            var menus = ds.Tables[0].ToList<Menu>().Where(x => x.GroupId == null).ToList();
            menus.ForEach(x =>
            {
                if (x.IsGroup)
                {
                    x.ChildMenus = GetGroupItemList((int)x.Id);
                }
            });
            return menus;
        }

        public List<Menu> GetGroupList(bool hasChildList = false)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetMenus", new Dictionary<string, object>() { { "IsGroup", true } });
            var menus = ds.Tables[0].ToList<Menu>();
            if (hasChildList)
            {
                menus.ForEach(x => x.ChildMenus = GetGroupItemList(x.Id));
            }
            return menus;
        }

        public List<Menu> GetGroupItemList(int GroupId)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetMenus", new Dictionary<string, object>() { { "GroupId", GroupId } });
            var menus = ds.Tables[0].ToList<Menu>();
            return menus;
        }

        public List<Menu> GetUserAccessList(string UserId)
        {
            var menus = GetParentList().OrderBy(x => x.MenuOrder).ToList();
            if (!string.IsNullOrEmpty(UserId))
            {
                var userAccessMenus = GetUserAccessMenus(UserId);
                menus.ForEach(x =>
                {
                    var childMenus = GetChildList(x.Id).Where(x => userAccessMenus.Select(uam => uam.MenuId).Contains(x.Id) && x.IsActive).OrderBy(x => x.MenuOrder).ToList();
                    x.ChildMenus = childMenus;
                });
            }
            else
            {
                menus.ForEach(x =>
                {
                    var childMenus = GetChildList(x.Id).OrderBy(x => x.MenuOrder).ToList();
                    x.ChildMenus = childMenus;
                });
            }
            return menus;
        }

        public List<UserAccessMenu> GetUserAccessMenus(string UserId = null, int? MenuId = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_connectionString, "SP_GetUserAccessMenus", new Dictionary<string, object>() { { "UserId", UserId }, { "MenuId", MenuId } });
            var userAccessMenus = ds.Tables[0].ToList<UserAccessMenu>();
            userAccessMenus.ForEach(x => x.Selected = true);
            return userAccessMenus;
        }

        public override async Task<bool> DeleteAsync(int Id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var menu = await _context.Menus.FindAsync(Id);
                    if (menu == null)
                    {
                        return false;
                    }
                    menu.IsDelete = true;
                    if (menu.ParentId == null)
                    {
                        var childMenus = _context.Menus.Where(x => x.ParentId == menu.Id);
                        foreach (var item in childMenus)
                        {
                            item.ParentId = null;
                        }
                    }
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
