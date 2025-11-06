using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MSIS_HMS.Core.Entities;

namespace MSIS_HMS.Interfaces
{
    public interface IBatchService
    {
        List<Batch> GetAll();
        Batch Get(int Id);
        SelectList GetSelectListItems(int? BatchId = null);
    }
}
