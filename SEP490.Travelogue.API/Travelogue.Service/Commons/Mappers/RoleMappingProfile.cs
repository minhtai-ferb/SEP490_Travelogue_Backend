using AutoMapper;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.RoleModels.Responses;

namespace Travelogue.Service.Commons.Mappers;
public class RoleMappingProfile : Profile
{
    public RoleMappingProfile()
    {
        CreateMap<Role, RoleResponseModel>().ReverseMap();
    }
}
