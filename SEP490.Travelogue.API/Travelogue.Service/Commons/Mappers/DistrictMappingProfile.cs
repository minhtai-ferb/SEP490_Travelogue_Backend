using AutoMapper;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.DistrictModels;

namespace Travelogue.Service.Commons.Mappers;
public class DistrictMappingProfile : Profile
{
    public DistrictMappingProfile()
    {
        CreateMap<DistrictCreateModel, District>().ReverseMap();
        CreateMap<DistrictCreateWithMediaFileModel, District>().ReverseMap();
        CreateMap<DistrictUpdateModel, District>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<DistrictUpdateWithMediaFileModel, District>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<DistrictDataModel, District>().ReverseMap();
    }
}