using AutoMapper;
using Template.Database.Models;
using Template.Service.Dto;

namespace Template.Service.Profiles;

public class InstituteProfile : Profile
{
    public InstituteProfile()
    {
        CreateMap<Institute, InstituteDto>();
    }
}