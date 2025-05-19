using AutoMapper;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.ExperienceModels;
using Travelogue.Service.BusinessModels.MediaModel;

namespace Travelogue.Service.Commons.Mappers;
public class ExperienceMappingProfile : Profile
{
    public ExperienceMappingProfile()
    {
        CreateMap<ExperienceCreateModel, Experience>().ReverseMap();
        CreateMap<ExperienceUpdateModel, Experience>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<ExperienceDataModel, Experience>().ReverseMap();

        CreateMap<ExperienceMedia, MediaResponse>();
    }
}