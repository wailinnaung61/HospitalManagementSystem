using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSIS_HMS.Services
{
    public class WarehouseService:IWarehouseService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IUserService _userService;
        public WarehouseService(UserManager<ApplicationUser> userManager, IWarehouseRepository warehouseRepository, IUserService userService)
        {
            _userManager = userManager;
            _warehouseRepository = warehouseRepository;
            _userService = userService;
        }
        public List<Warehouse> GetAll()
        {
            return _warehouseRepository.GetAll();
        }
        public SelectList GetSelectListItems(int? warehouseId = null)
        {
            var warehouses = _warehouseRepository.GetAll();
            return new SelectList(warehouses, "Id", "Name", warehouseId);
        }
        public SelectList GetSelectListItemsByBranch(int? branchId=null,int? warehouseId = null)
        {
            var warehouses = _warehouseRepository.GetAll(branchId);
            return new SelectList(warehouses, "Id", "Name", warehouseId);
        }
    }
}
