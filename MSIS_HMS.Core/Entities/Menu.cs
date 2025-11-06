using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using MSIS_HMS.Core.Entities.Base;
using MSIS_HMS.Core.Repositories;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    [Table("Menu")]
    public class Menu : Entity
    {
        public Menu()
        {
        }

        [Required]
        public string Name { get; set; }
        [Required]
        public string DisplayName { get; set; }
        public string Image { get; set; }
        [Required]
        public string DisplayName_mm { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Params { get; set; }
        public string Description { get; set; }
        public bool IsDashboard { get; set; }
        [Required]
        public string Icon { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }
        public int? MenuOrder { get; set; }
        public int? ParentId { get; set; }
        public bool IsGroup { get; set; }
        public int? GroupId { get; set; }
        public int ModuleId { get; set; }
        public bool IsBranchId {get;set;}
        public bool IsOutletId { get; set; }
        public bool IsDoctorId { get; set; }
        
        [NotMapped]
        [SkipProperty]
        [Required]
        public string ImageContent { get; set; }
        [NotMapped]
        [SkipProperty]
        public IFormFile ImageFile { get; set; }
        
        [NotMapped]
        [SkipProperty]
        public string ModuleName { get; set; }
        [NotMapped]
        [SkipProperty]
        public bool Selected { get; set; }
        [NotMapped]
        [SkipProperty]
        public ICollection<Menu> ChildMenus { get; set; }

        [SkipProperty]
        public Module Module { get; set; }
        [SkipProperty]
        public ICollection<UserAccessMenu> UserAccessMenus { get; set; }

        public Module GetModule(IModuleRepository moduleRepository, int ModuleId)
        {
            return moduleRepository.Get(ModuleId);
        }
    }
}
