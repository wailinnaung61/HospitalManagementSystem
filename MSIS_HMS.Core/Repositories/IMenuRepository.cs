using System;
using System.Collections.Generic;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories.Base;

namespace MSIS_HMS.Core.Repositories
{
    public interface IMenuRepository : IRepository<Menu>
    {
        List<Menu> GetParentList(bool hasChildList = false);
        List<Menu> GetChildList(int ParentId);
        List<Menu> GetGroupList(bool hasChildList = false);
        List<Menu> GetGroupItemList(int GroupId);
        List<Menu> GetUserAccessList(string UserId);
        List<UserAccessMenu> GetUserAccessMenus(string UserId = null, int? MenuId = null);
    }
}
