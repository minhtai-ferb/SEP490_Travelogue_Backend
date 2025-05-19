using AutoMapper;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.HotelModels;

namespace Travelogue.Service.Commons.Mappers;
public class HotelMappingProfile : Profile
{
    public HotelMappingProfile()
    {
        CreateMap<HotelCreateModel, Hotel>().ReverseMap();
        CreateMap<HotelUpdateModel, Hotel>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<HotelUpdateWithMediaFileModel, Hotel>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<HotelDataModel, Hotel>().ReverseMap();
    }
}