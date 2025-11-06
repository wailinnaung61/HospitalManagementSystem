using System;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Models;

namespace MSIS_HMS.Models
{
    public class AuthenticateResponse
    {
        public AuthenticateResponse(ApplicationUser user, string token)
        {
            Id = user.Id;
            UserName = user.UserName;
            Token = token;
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
    }
}
