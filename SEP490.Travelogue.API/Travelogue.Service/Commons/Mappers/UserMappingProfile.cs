using AutoMapper;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.UserModels.Requests;
using Travelogue.Service.BusinessModels.UserModels.Responses;

namespace Travelogue.Service.Commons.Mappers;
public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, RegisterModel>().ReverseMap();
        CreateMap<User, UserResponseModel>().ReverseMap();
        CreateMap<User, UserUpdateModel>()
            .ReverseMap()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<User, GetCurrentUserResponse>().ReverseMap();
    }
}
