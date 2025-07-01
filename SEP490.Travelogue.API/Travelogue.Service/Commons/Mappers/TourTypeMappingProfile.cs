using AutoMapper;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.TourTypeModels;

namespace Travelogue.Service.Commons.Mappers;

public class TourTypeMappingProfile : Profile
{
    public TourTypeMappingProfile()
    {
        CreateMap<TourTypeCreateModel, TourType>().ReverseMap();
        CreateMap<TourTypeUpdateModel, TourType>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<TourTypeDataModel, TourType>().ReverseMap();
    }
}