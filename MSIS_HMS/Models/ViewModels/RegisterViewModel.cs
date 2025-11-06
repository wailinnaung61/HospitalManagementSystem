using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MSIS_HMS.Core.Entities;

namespace MSIS_HMS.Models.ViewModels
{
    public class RegisterViewModel
    {
        public RegisterViewModel()
        {
        }

        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        //[Required(ErrorMessage = "Confirm Password is required")]
        //[StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        //[DataType(DataType.Password)]
        //[Compare("Password")]
        //public string ConfirmPassword { get; set; }

        [Required]
        public string Role { get; set; }

        public int? DoctorId { get; set; }

        public int? OutletId { get; set; }

        public int? BranchId { get; set; }

        public List<UserAccessMenu> UserAccessMenus { get; set; }
    }
}
