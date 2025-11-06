using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Interfaces;

namespace MSIS_HMS.Services
{
    public class ItemService : IItemService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IItemRepository _itemRepository;
        private readonly IUserService _userService;
        private readonly IBranchService _branchService;
        private readonly ClaimsPrincipal _user;

        public ItemService(UserManager<ApplicationUser> userManager, IItemRepository itemRepository, IUserService userService, IBranchService branchService, ClaimsPrincipal user)
        {
            _userManager = userManager;
            _itemRepository = itemRepository;
            _userService = userService;
            _branchService = branchService;
            _user = user;
        }

        public List<Item> GetAll()
        {
            return _itemRepository.GetAll(_branchService.GetBranchIdByUser()).OrderBy(x => x.Name).ToList();
        }

        public Item Get(int Id)
        {
            return _itemRepository.Get(Id);
        }

        public SelectList GetSelectListItems(int? itemId = null)
        {
            var items = GetAll();
            List<object> _items = new List<object>();
            foreach (var item in items)
                _items.Add(new
                {
                    Id = item.Id,
                    Name = item.Name + " (" + item.Code + ")"
                });
            return new SelectList(_items, "Id", "Name", itemId);
        }
    }
}
