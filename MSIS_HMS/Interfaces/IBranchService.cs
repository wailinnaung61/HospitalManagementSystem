using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Models;

namespace MSIS_HMS.Interfaces
{
    public interface IBranchService
    {
        List<Branch> GetAll();
        SelectList GetSelectListItems(int? branchId = null);
        string GetBranchName(int id);
        int? GetBranchIdByUser();
        Branch GetBranchByUser();
        Branch GetBranchById(int id);
        VoucherSetting GetVoucherSetting(VoucherTypeEnum type);
        string GetVoucherNo(VoucherTypeEnum type);
        string GetVoucherNo(VoucherTypeEnum type, string defaultValue);
        Task IncreaseVoucherNo(VoucherTypeEnum type);
    }
}
