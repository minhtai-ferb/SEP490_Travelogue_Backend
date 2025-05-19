using AutoMapper;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.RestaurantModels;

namespace Travelogue.Service.Commons.Mappers;
public class RestaurantMappingProfile : Profile
{
    public RestaurantMappingProfile()
    {
        CreateMap<RestaurantCreateModel, Restaurant>().ReverseMap();
        CreateMap<RestaurantUpdateModel, Restaurant>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<RestaurantUpdateWithMediaFileModel, Restaurant>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<RestaurantDataModel, Restaurant>().ReverseMap();
    }
}