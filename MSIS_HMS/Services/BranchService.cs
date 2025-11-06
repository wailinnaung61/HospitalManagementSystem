using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Interfaces;
using MSIS_HMS.Models;

namespace MSIS_HMS.Services
{
    public class BranchService : IBranchService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBranchRepository _branchRepository;
        private readonly IUserService _userService;
        private readonly ClaimsPrincipal _user;

        public BranchService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IBranchRepository branchRepository, IUserService userService, ClaimsPrincipal user)
        {
            _context = context;
            _userManager = userManager;
            _branchRepository = branchRepository;
            _userService = userService;
            _user = user;
        }

        public List<Branch> GetAll()
        {
            return _branchRepository.GetAll();
        }

        public SelectList GetSelectListItems(int? branchId = null)
        {
            var branches = _branchRepository.GetAll();
            return new SelectList(branches, "Id", "Name", branchId ?? GetBranchIdByUser());
        }

        public string GetBranchName(int id)
        {
            var branch = _branchRepository.Get(id);
            return branch.Name;
        }

        public Branch GetBranchById(int id)
        {
            var branch = _branchRepository.Get(id);
            return branch;
        }


        public int? GetBranchIdByUser()
        {
            var userId = _userManager.GetUserId(_user);
            var user = _userService.Get(userId);
            return user.BranchId;
        }

        public Branch GetBranchByUser()
        {
            var userId = _userManager.GetUserId(_user);
            var user = _userService.Get(userId);
            return _branchRepository.Get((int)user.BranchId);
        }

        public VoucherSetting GetVoucherSetting(VoucherTypeEnum type)
        {
            var branch = GetBranchByUser();
            switch (type)
            {
                case VoucherTypeEnum.Pharmacy:
                    return new VoucherSetting(branch.UseVoucherFormatForOrder, branch.VoucherFormatForOrder, branch.VoucherNoForOrder);
                case VoucherTypeEnum.Purchase:
                    return new VoucherSetting(branch.UseVoucherFormatForPurchase, branch.VoucherFormatForPurchase, branch.VoucherNoForPurchase);
                case VoucherTypeEnum.Deliver:
                    return new VoucherSetting(branch.UseVoucherFormatForDeliverOrder, branch.VoucherFormatForDeliverOrder, branch.VoucherNoForDeliverOrder);
                case VoucherTypeEnum.Batch:
                    return new VoucherSetting(branch.UseBatchNoFormat, branch.VoucherFormatForDeliverOrder, branch.VoucherNoForDeliverOrder);
                case VoucherTypeEnum.Patient:
                    return new VoucherSetting(true, branch.PatientRegFormat, branch.PatientRegNo);
                case VoucherTypeEnum.Visit:
                    return new VoucherSetting(true, branch.VisitNoFormat, branch.VisitNo);
                case VoucherTypeEnum.IPD:
                    return new VoucherSetting(branch.UseVoucherFormatForIPD, branch.VoucherFormatForIPD, branch.VoucherNoForIPD);
                case VoucherTypeEnum.Lab:
                    return new VoucherSetting(branch.UseVoucherFormatForLab, branch.VoucherFormatForLab, branch.VoucherNoForLab);
                case VoucherTypeEnum.LabResult:
                    return new VoucherSetting(branch.UseVoucherFormatForLabResult, branch.VoucherFormatForLabResult, branch.VoucherNoForLabResult);
                case VoucherTypeEnum.Imaging:
                    return new VoucherSetting(branch.UseVoucherFormatForImaging, branch.VoucherFormatForImaging, branch.VoucherNoForImaging);
                default: return null;
            }
        }

        public string GetVoucherNo(VoucherTypeEnum type)
        {
            var voucherSetting = GetVoucherSetting(type);
            if (voucherSetting == null) return null;
            if (!voucherSetting.UseVoucherFormat) return null;
            var format = voucherSetting.Format;
            var number = voucherSetting.Number;
            if(string.IsNullOrEmpty(format))
            {
                return null;
            }
            number += 1;
            return number.ToString(format);
        }

        public string GetVoucherNo(VoucherTypeEnum type, string defaultValue)
        {
            return GetVoucherNo(type) ?? defaultValue;
        }

        public async Task IncreaseVoucherNo(VoucherTypeEnum type)
        {
            var voucherSetting = GetVoucherSetting(type);
            var branch = await _context.Branches.FindAsync((int)GetBranchIdByUser());
            if(branch != null)
            {
                switch (type)
                {
                    case VoucherTypeEnum.Pharmacy:
                        if(branch.UseVoucherFormatForOrder) branch.VoucherNoForOrder = voucherSetting.Number + 1; break;
                    case VoucherTypeEnum.Purchase:
                        if (branch.UseVoucherFormatForPurchase) branch.VoucherNoForPurchase = voucherSetting.Number + 1; break;
                    case VoucherTypeEnum.Deliver:
                        if (branch.UseVoucherFormatForDeliverOrder) branch.VoucherNoForDeliverOrder = voucherSetting.Number + 1; break;
                    case VoucherTypeEnum.Batch:
                        if (branch.UseBatchNoFormat) branch.BatchNo = voucherSetting.Number + 1; break;
                    case VoucherTypeEnum.Patient:
                        branch.PatientRegNo = voucherSetting.Number + 1; break;
                    case VoucherTypeEnum.Visit:
                        branch.VisitNo = voucherSetting.Number + 1; break;
                    case VoucherTypeEnum.IPD:
                        if (branch.UseVoucherFormatForIPD) branch.VoucherNoForIPD = voucherSetting.Number + 1; break;
                    case VoucherTypeEnum.Lab:
                        if (branch.UseVoucherFormatForLab) branch.VoucherNoForLab = voucherSetting.Number + 1; break;
                    case VoucherTypeEnum.LabResult:
                        if (branch.UseVoucherFormatForLabResult) branch.VoucherNoForLabResult = voucherSetting.Number + 1; break;
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}
