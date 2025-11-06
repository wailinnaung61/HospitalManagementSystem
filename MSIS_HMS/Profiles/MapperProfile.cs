using System;
using AutoMapper;
using MSIS_HMS.Core.Entities;
using MSIS_HMS.Models.DTOs;

namespace MSIS_HMS.Profiles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ApplicationUser, UserDTO>();
        }
    }
}
