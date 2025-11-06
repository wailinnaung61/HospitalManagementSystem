using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Helpers;
using MSIS_HMS.Models;
using System.Data;
using MSIS_HMS.Infrastructure.Helpers;
using MSIS_HMS.Infrastructure.Data;
using MSIS_HMS.Models.DTOs;
using MSIS_HMS.Models.ViewModels;
using System.Linq;
using AutoMapper;
using MSIS_HMS.Infrastructure.Interfaces;
using MSIS_HMS.Interfaces;

namespace MSIS_HMS.Services
{
    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfigService _configService;

        public UserService(ApplicationDbContext context, IOptions<AppSettings> appSettings, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IMapper mapper, IConfigService config)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _configService = config;
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {
            var result = await Login(model);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                var token = generateJwtToken(user);
                return new AuthenticateResponse(user, token);
            }
            return null;
        }

        public async Task<Microsoft.AspNetCore.Identity.SignInResult> Login(AuthenticateRequest model)
        {

            return await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, model.LockoutOnFailure);
        }

        public async void Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<ApplicationUser> Register(RegisterViewModel model)
        {
            var createUser = new ApplicationUser
            {
                Name = model.Name,
                UserName = model.Email,
                Email = model.Email,
                DoctorId = model.DoctorId,
                OutletId = model.OutletId,
                BranchId = model.BranchId,
                UserAccessMenus = model.UserAccessMenus.Where(x => x.Selected).ToList()
            };
            var result = await _userManager.CreateAsync(createUser, model.Password);
            if (result.Succeeded)
            {
                var role_result = await _userManager.AddToRoleAsync(createUser, model.Role);
                if (role_result.Succeeded)
                {
                    return createUser;
                }
            }
            return null;
        }

        public List<UserDTO> GetAll(string RoleId = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_configService.GetConnectionString(), "SP_GetUsers", new Dictionary<string, object>() { { "RoleId", RoleId } });
            var users = ds.Tables[0].ToList<UserDTO>();
            return users;
        }

        public UserDTO Get(string Id)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_configService.GetConnectionString(), "SP_GetUsers", new Dictionary<string, object>() { { "UserId", Id } });
            var user = ds.Tables[0].ToList<UserDTO>()[0];
            return user;
        }

        public UserDTO Get(ClaimsPrincipal User)
        {
            var Id = _userManager.GetUserId(User);
            DataSet ds = EfCoreExtensions.Execute_SP(_configService.GetConnectionString(), "SP_GetUsers", new Dictionary<string, object>() { { "UserId", Id } });
            var user = ds.Tables[0].ToList<UserDTO>()[0];
            return user;
        }

        public List<UserAccessMenu> GetUserAccessMenus(string UserId = null, int? MenuId = null)
        {
            DataSet ds = EfCoreExtensions.Execute_SP(_configService.GetConnectionString(), "SP_GetUserAccessMenus", new Dictionary<string, object>() { { "UserId", UserId }, { "MenuId", MenuId } });
            var userAccessMenus = ds.Tables[0].ToList<UserAccessMenu>();
            userAccessMenus.ForEach(x => x.Selected = true);
            return userAccessMenus;
        }

        public async Task<ApplicationUser> GetByIdAsync(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            return user;
        }

        public async Task<UserDTO> UpdateAsync(RegisterViewModel model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(model.Id);
                    var oldRoleName = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

                    if (oldRoleName != model.Role)
                    {
                        await _userManager.RemoveFromRoleAsync(user, oldRoleName);
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }
                    if (user.UserName != model.Email)
                    {
                        user.UserName = model.Email;
                        user.Email = model.Email;
                    }
                    if (!string.IsNullOrEmpty(model.NewPassword))
                    {
                        user.PasswordHash = GetPasswordHash(user, model.NewPassword);
                    }

                    user.Name = model.Name;
                    user.DoctorId = model.DoctorId;
                    user.OutletId = model.OutletId;
                    user.BranchId = model.BranchId;

                    var newUserAccessMenus = model.UserAccessMenus.Where(x => x.Selected).ToList();
                    var oldUserAccessMenus = GetUserAccessMenus(user.Id);
                    if (!(oldUserAccessMenus.Select(x => x.MenuId).ToList().All(newUserAccessMenus.Select(x => x.MenuId).ToList().Contains) && newUserAccessMenus.Select(x => x.MenuId).ToList().All(oldUserAccessMenus.Select(x => x.MenuId).ToList().Contains)))
                    {
                        _context.UserAccessMenus.RemoveRange(oldUserAccessMenus);
                        newUserAccessMenus.ToList().ForEach(x => x.UserId = user.Id);
                        await _context.UserAccessMenus.AddRangeAsync(newUserAccessMenus);
                    }

                    await _userManager.UpdateAsync(user);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return Get(user.Id);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    await transaction.RollbackAsync();
                    return null;
                }
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var rolesForUser = await _userManager.GetRolesAsync(user);

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (rolesForUser.Count() > 0)
                    {
                        foreach (var item in rolesForUser.ToList())
                        {
                            // item should be the name of the role
                            var result = await _userManager.RemoveFromRoleAsync(user, item);
                        }
                    }

                    var userAccessMenus = GetUserAccessMenus(user.Id);
                    if (userAccessMenus.Count() > 0)
                    {
                        _context.UserAccessMenus.RemoveRange(userAccessMenus);
                    }

                    await _userManager.DeleteAsync(user);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }

        public RegisterViewModel ConvertModel(UserDTO user)
        {
            var userAccessMenus = GetUserAccessMenus(user.Id);
            return new RegisterViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                BranchId = user.BranchId,
                DoctorId = user.DoctorId,
                OutletId = user.OutletId,
                Name = user.Name,
                Role = user.RoleName,
                UserAccessMenus = userAccessMenus
            };
        }

        public UserDTO ConvertModel(RegisterViewModel model)
        {
            var role = _context.Roles.Where(x => x.Name == model.Role).FirstOrDefault();
            var roleId = role != null ? role.Id : "";
            return new UserDTO()
            {
                Id = model.Id,
                Email = model.Email,
                BranchId = model.BranchId,
                DoctorId = model.DoctorId,
                OutletId = model.OutletId,
                Name = model.Name,
                RoleId = roleId,
                RoleName = model.Role,
                UserAccessMenus = model.UserAccessMenus.Where(x => x.Selected).ToList()
            };
        }

        private string GetPasswordHash(ApplicationUser user, string pass)
        {
            var password = new PasswordHasher<ApplicationUser>();
            var hashed = password.HashPassword(user, pass);
            return hashed;
        }

        private string generateJwtToken(ApplicationUser user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("id", user.Id),
                    new Claim(ClaimTypes.Email, user.Email),
                    //new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
