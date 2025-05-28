using AutoMapper;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.TripPlanModels;

namespace Travelogue.Service.Commons.Mappers;

public class TripPlanMappingProfile : Profile
{
    public TripPlanMappingProfile()
    {
        CreateMap<TripPlanCreateModel, TripPlan>().ReverseMap();
        CreateMap<TripPlanUpdateModel, TripPlan>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<TripPlanDataModel, TripPlan>().ReverseMap();
    }
}