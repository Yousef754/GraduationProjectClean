using AutoMapper;
using ECommerce.Domain.Entities.AppUser;
using ECommerce.Shared.DTOs.AppUserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterUserDto, AppUser>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // هيتم هاش الباسورد في Service
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => "User")); // كل تسجيل User
        }
    }
}
