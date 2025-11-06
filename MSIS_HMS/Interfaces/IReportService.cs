using MSIS_HMS.Core.Entities.DTOs;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSIS_HMS.Interfaces
{
    public interface IReportService
    {
         ExcelPackage WarehouseItemByExRemindDayexcelExport(List<WarehouseItemLocationDTO> warehouseItems);
    }
}
