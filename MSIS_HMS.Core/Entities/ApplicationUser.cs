using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using static MSIS_HMS.Core.Entities.Attributes.CustomAttribute;

namespace MSIS_HMS.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public int? DoctorId { get; set; }
        public int? OutletId { get; set; }
        public int? BranchId { get; set; }

        [SkipProperty]
        public Doctor Doctor { get; set; }
        [SkipProperty]
        public Outlet Outlet { get; set; }
        [SkipProperty]
        public Branch Branch { get; set; }

        [SkipProperty]
        public ICollection<UserAccessMenu> UserAccessMenus { get; set; }
    }
}
