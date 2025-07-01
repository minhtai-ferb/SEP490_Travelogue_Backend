using AutoMapper;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.TourModels;

namespace Travelogue.Service.Commons.Mappers;

public class TourMappingProfile : Profile
{
    public TourMappingProfile()
    {
        CreateMap<TourCreateModel, Tour>().ReverseMap();
        CreateMap<TourUpdateModel, Tour>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<TourDataModel, Tour>().ReverseMap();
        CreateMap<TourSchedule, TourScheduleModel>().ReverseMap();
        CreateMap<TourPlanLocation, TourPlanLocationModel>().ReverseMap();
    }
}