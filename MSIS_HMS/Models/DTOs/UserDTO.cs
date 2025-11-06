using System;
using MSIS_HMS.Core.Entities;

namespace MSIS_HMS.Models.DTOs
{
    public class UserDTO : ApplicationUser
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string OutletName { get; set; }
        public string BranchName { get; set; }
    }
}
