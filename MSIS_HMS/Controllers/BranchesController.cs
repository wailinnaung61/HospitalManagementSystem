using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Enums;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Infrastructure.Helpers;

using MSIS_HMS.Models;
using X.PagedList;

namespace MSIS_HMS.Controllers
{
    [Authorize(Roles = "Superadmin")]
    public class BranchesController : Controller
    {
        private readonly IBranchRepository _branchRepository;
        private readonly ILogger<BranchesController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Pagination _pagination;


        public BranchesController(IBranchRepository branchRepository, ILogger<BranchesController> logger,
            ApplicationDbContext context, UserManager<ApplicationUser> userManager,IOptions<Pagination> pagination)
        {
            _branchRepository = branchRepository;
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _pagination = pagination.Value;
        }

        // GET: Branches
        public ActionResult Index(int? page = 1,string BranchName=null,string BranchCode=null)
        {
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
                     
            var branches = _branchRepository.GetAll().Where(branch =>
                ((BranchName != null && branch.Name.Contains(BranchName)) || (BranchName == null && branch.Name != null)) &&
                ((BranchCode != null && branch.Code == BranchCode) || (BranchCode == null && branch.Code != null)));
            return View(branches.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }
        public void Initialize(Branch branch = null)
        {
            var states = _context.States.Where(x => x.IsDelete == false).ToList();
            ViewData["States"] = new SelectList(states, "Id", "Name",branch?.StateId);
            var cities = _context.Cities.Where(x => x.IsDelete == false).ToList();
            ViewData["Cities"] = new SelectList(cities, "Id", "Name",branch?.CityId);
            var townships = _context.Townships.Where(x => x.IsDelete == false).ToList();
            ViewData["Townships"] = new SelectList(townships, "Id", "Name",branch?.TownshipId);
            var countries = _context.Countries.Where(x => x.IsDelete == false).ToList();
            ViewData["Countries"] = new SelectList(countries, "Id", "Name",branch?.CountryId);           
            
            
        }
        // GET: Branches/Create
        public ActionResult Create()
        {
            Initialize();
            return View();
        }

        // POST: Branches/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Branch branch)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        branch = await _userManager.AddUserAndTimestamp(branch, User, DbEnum.DbActionEnum.Create);
                        //branch.CFFeeForHospital = branch.CFFeeForHospital / 100;
                        var _branch = await _branchRepository.AddAsync(branch);

                        if (_branch.IsMainBranch == true)
                        {
                            await RemoveMainBranchStatus(_branch.Id);
                        }

                        await transaction.CommitAsync();
                        TempData["notice"] = StatusEnum.NoticeStatus.Success;
                        _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Success));
                        return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.InnerException.Message);
                        Console.WriteLine(e.Message);
                        await transaction.RollbackAsync();
                        return View();
                    }

                }

            }
            Initialize();
            return View();
        }

        // GET: Branches/Edit/5
        public ActionResult Edit(int id)
        {
            var _branch = _branchRepository.Get(id);
            Initialize(_branch);
            return View(_branch);
        }

        // POST: Branches/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Branch branch)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // branch.CFFeeForHospital = branch.CFFeeForHospital / 100;
                      
                        branch = await _userManager.AddUserAndTimestamp(branch, User, DbEnum.DbActionEnum.Update);
                        var _branch = await _branchRepository.UpdateAsync(branch);

                        if (_branch.IsMainBranch == true)
                        {
                             await RemoveMainBranchStatus(_branch.Id);
                        }

                        await transaction.CommitAsync();
                        TempData["notice"] = StatusEnum.NoticeStatus.Edit;
                        _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Edit));
                        return RedirectToAction("Index");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.InnerException.Message);
                        Console.WriteLine(e.Message);
                        await transaction.RollbackAsync();
                        return View();
                    }
                }
            }
            return View();
        }

        // GET: Branches/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
         
            try
            {
                var _branch = await _branchRepository.DeleteAsync(id);
                TempData["notice"] = StatusEnum.NoticeStatus.Delete;
                _logger.LogInformation(Infrastructure.Enums.EnumExtension.ToDescription(StatusEnum.NoticeStatus.Delete));
               
            }
            catch(Exception e)
            {
                _logger.LogError(e.InnerException.Message);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<bool> RemoveMainBranchStatus(int MainBranchId)
        {
            try
            {

                //var _branches =  _context.Branches.Where(x => x.Id != MainBranchId && x.IsDelete == false)
                //       .ToList();
                var _branches = _branchRepository.GetNonMainBranch(MainBranchId);
                foreach (var _branch in _branches)
                {
                    _branch.IsMainBranch = false;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
           
        }

        public IActionResult GetState(int? countryId)
        {
            var states = _context.States.Where(x => x.CountryId == countryId && x.IsDelete == false).ToList();
            return Ok(states);
        }

        public IActionResult GetCity(int? stateId)
        {
            var cities = _context.Cities.Where(x => x.StateId == stateId && x.IsDelete == false).ToList();
            return Ok(cities);
        }
        public IActionResult GetTownship(int? cityId)
        {
            var townships = _context.Townships.Where(x => x.CityId == cityId && x.IsDelete == false).ToList();
            return Ok(townships);
        }
    }
}