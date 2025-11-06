using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using MSIS_HMS.Core.Repositories;
using MSIS_HMS.Infrastructure.Enums;
using MSIS_HMS.Models;
using X.PagedList;

namespace MSIS_HMS.Controllers
{
    [Authorize(Roles = "Superadmin")]
    public class ModulesController : Controller
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly Pagination _pagination;

        public ModulesController(IModuleRepository moduleRepository,IOptions<Pagination> pagination)
        {
            _moduleRepository = moduleRepository;
            _pagination = pagination.Value;
        }

        // GET: Modules
        public IActionResult Index(int? page=1,string ModuleName=null,string DisplayName=null,string Code=null)
        {
            var pageSize = _pagination.PageSize;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            
            var modules = _moduleRepository.GetAll().Where(module=>
                    ((ModuleName != null && module.Name.Contains(ModuleName)) || (ModuleName == null && module.Name != null)) &&
                    ((Code != null && module.Code == Code) || (Code == null && module.Code != null)) &&
                    ((DisplayName != null && module.DisplayName.Contains(DisplayName)) || (DisplayName == null && module.DisplayName != null)));

            return View(modules.OrderByDescending(x => x.UpdatedAt).ToList().ToPagedList((int)page, pageSize));
        }
    }
}