using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MSIS_HMS.Core.Entities;

namespace MSIS_HMS.Interfaces
{
    public interface IItemService
    {
        List<Item> GetAll();
        Item Get(int Id);
        SelectList GetSelectListItems(int? ItemId = null);
    }
}
