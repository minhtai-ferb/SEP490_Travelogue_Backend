using AutoMapper;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.TypeEventModels;

namespace Travelogue.Service.Commons.Mappers;
public class TypeEventMappingProfile : Profile
{
    public TypeEventMappingProfile()
    {
        CreateMap<TypeEventCreateModel, TypeEvent>().ReverseMap();
        CreateMap<TypeEventUpdateModel, TypeEvent>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<TypeEventDataModel, TypeEvent>().ReverseMap();
    }
}