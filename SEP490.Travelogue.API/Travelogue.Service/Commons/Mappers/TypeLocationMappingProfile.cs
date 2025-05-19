using AutoMapper;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.TypeLocationModels;

namespace Travelogue.Service.Commons.Mappers;
public class TypeLocationMappingProfile : Profile
{
    public TypeLocationMappingProfile()
    {
        CreateMap<TypeLocationCreateModel, TypeLocation>().ReverseMap();
        CreateMap<TypeLocationUpdateModel, TypeLocation>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<TypeLocationDataModel, TypeLocation>().ReverseMap();
    }
}