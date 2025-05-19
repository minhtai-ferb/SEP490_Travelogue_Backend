using AutoMapper;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.TypeExperienceModels;

namespace Travelogue.Service.Commons.Mappers;
public class TypeExperienceMappingProfile : Profile
{
    public TypeExperienceMappingProfile()
    {
        CreateMap<TypeExperienceCreateModel, TypeExperience>().ReverseMap();
        CreateMap<TypeExperienceUpdateModel, TypeExperience>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<TypeExperienceDataModel, TypeExperience>().ReverseMap();
    }
}