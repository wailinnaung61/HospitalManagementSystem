using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Models;
using MSIS_HMS.Models.DTOs;
using MSIS_HMS.Models.ViewModels;

namespace MSIS_HMS.Interfaces
{
    public interface IUserService
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model);
        Task<Microsoft.AspNetCore.Identity.SignInResult> Login(AuthenticateRequest model);
        void Logout();
        Task<ApplicationUser> Register(RegisterViewModel model);
        List<UserDTO> GetAll(string RoleId = null);
        UserDTO Get(string Id);
        UserDTO Get(ClaimsPrincipal User);
        List<UserAccessMenu> GetUserAccessMenus(string UserId = null, int? MenuId = null);
        RegisterViewModel ConvertModel(UserDTO user);
        Task<UserDTO> UpdateAsync(RegisterViewModel model);
        Task<bool> DeleteAsync(string id);
        Task<ApplicationUser> GetByIdAsync(string Id);
    }
}
