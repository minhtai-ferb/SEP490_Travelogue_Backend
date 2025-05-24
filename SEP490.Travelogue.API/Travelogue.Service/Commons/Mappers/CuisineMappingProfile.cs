using AutoMapper;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.CuisineModels;

namespace Travelogue.Service.Commons.Mappers;
public class CuisineMappingProfile : Profile
{
    public CuisineMappingProfile()
    {
        CreateMap<CuisineCreateModel, Cuisine>().ReverseMap();
        CreateMap<CuisineUpdateModel, Cuisine>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<CuisineUpdateWithMediaFileModel, Cuisine>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<CuisineDataModel, Cuisine>().ReverseMap();
    }
}