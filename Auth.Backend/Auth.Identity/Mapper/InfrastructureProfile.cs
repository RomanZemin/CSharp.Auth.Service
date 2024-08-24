﻿using AutoMapper;
using Auth.Application.DTOs;
using Auth.Infrastructure.Identity.Models;

namespace Auth.Infrastructure.Identity.Mapper
{
    public class InfrastructureIdentityProfile : Profile
    {
        public InfrastructureIdentityProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserDto>()
                .ReverseMap();
        }
    }
}
