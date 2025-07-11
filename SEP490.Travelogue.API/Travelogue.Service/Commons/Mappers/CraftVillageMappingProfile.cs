using AutoMapper;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.CraftVillageModels;

namespace Travelogue.Service.Commons.Mappers;

public class CraftVillageMappingProfile : Profile
{
    public CraftVillageMappingProfile()
    {
        CreateMap<CraftVillageCreateModel, CraftVillage>().ReverseMap();
        CreateMap<CraftVillageUpdateDto, CraftVillage>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<CraftVillageUpdateWithMediaFileModel, CraftVillage>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<CraftVillageDataModel, CraftVillage>().ReverseMap();
    }
}