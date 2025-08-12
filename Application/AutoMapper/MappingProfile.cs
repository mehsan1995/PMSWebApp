using Application.DTOs;
using AutoMapper;
using DAL.Models;


namespace Application.AutoMapper
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email)); // 👈 example
            CreateMap<ApplicationUser, UserDto>();
            CreateMap<ApplicationRole, RoleDto>().ReverseMap();
            CreateMap<Departments, DepartmentsDto>().ReverseMap();
            CreateMap<Tenants, TenantsDto>().ReverseMap();
            CreateMap<DepartmentType, DepartmentTypeDto>().ReverseMap();
            CreateMap<DepartmentUsers, DepartmentUsersDto>().ReverseMap();
            CreateMap<TenantSettings, TenantSettingsDto>().ReverseMap();
        }
    }
}
