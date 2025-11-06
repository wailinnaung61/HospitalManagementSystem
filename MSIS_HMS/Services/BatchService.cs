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
    public class BatchService : IBatchService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBatchRepository _batchRepository;
        private readonly IUserService _userService;
        private readonly IBranchService _branchService;
        private readonly ClaimsPrincipal _user;

        public BatchService(UserManager<ApplicationUser> userManager, IBatchRepository batchRepository, IUserService userService, IBranchService branchService, ClaimsPrincipal user)
        {
            _userManager = userManager;
            _batchRepository = batchRepository;
            _userService = userService;
            _branchService = branchService;
            _user = user;
        }

        public List<Batch> GetAll()
        {
            return _batchRepository.GetAll(_branchService.GetBranchIdByUser()).OrderBy(x => x.Name).ToList();
        }

        public Batch Get(int Id)
        {
            return _batchRepository.Get(Id);
        }

        public SelectList GetSelectListItems(int? batchId = null)
        {
            var batches = GetAll();
            List<object> _batches = new List<object>();
            foreach (var batch in batches)
                _batches.Add(new
                {
                    Id = batch.Id,
                    Name = batch.Name + " (" + batch.Code + ")"
                });
            return new SelectList(_batches, "Id", "Name", batchId);
        }
    }
}
