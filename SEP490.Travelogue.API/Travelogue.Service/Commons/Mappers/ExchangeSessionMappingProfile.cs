using AutoMapper;
using Travelogue.Repository.Entities;
using Travelogue.Service.BusinessModels.ExchangeModels;
using Travelogue.Service.BusinessModels.ExchangeSessionModels;
using Travelogue.Service.BusinessModels.HotelModels;

namespace Travelogue.Service.Commons.Mappers;

public class ExchangeSessionMappingProfile : Profile
{
    public ExchangeSessionMappingProfile()
    {
        // CreateMap<HotelCreateModel, Hotel>().ReverseMap();
        // CreateMap<HotelUpdateModel, Hotel>()
        //     .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<ExchangeSessionDataModel, TripPlanExchangeSession>().ReverseMap();
        CreateMap<ExchangeSessionDataDetailModel, TripPlanExchangeSession>().ReverseMap();

        CreateMap<TripPlanExchangeSession, ExchangeSessionDataDetailModel>()
            .ForMember(dest => dest.TripPlanName, opt => opt.MapFrom(src => src.TripPlan.Name))
            .ForMember(dest => dest.TourGuideName, opt => opt.MapFrom(src => src.TourGuide.User.FullName))
            .ForMember(dest => dest.ExchangeData, opt => opt.MapFrom(src =>
                src.Exchanges
                    .OrderByDescending(e => e.RequestedAt)
                    .FirstOrDefault() ?? new TripPlanExchange())); // fallback nếu null

        CreateMap<TripPlanExchange, ExchangeDataModel>()
            .ForMember(dest => dest.ExchangeId, opt => opt.MapFrom(src => src.Id));
        // .ForMember(dest => dest.TripPlanVersionId, opt => opt.MapFrom(src => src.TripPlanVersionId))
        // .ForMember(dest => dest.SuggestedTripPlanVersionId, opt => opt.MapFrom(src => src.SuggestedTripPlanVersionId))
        // .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
        // .ForMember(dest => dest.ExchangeSessionStatus, opt => opt.MapFrom(src => src.Status.ToString()))
        // .ForMember(dest => dest.RequestedAt, opt => opt.MapFrom(src => src.RequestedAt))
        // .ForMember(dest => dest.UserRespondedAt, opt => opt.MapFrom(src => src.RespondedAt))
        // .ForMember(dest => dest.UserResponseMessage, opt => opt.MapFrom(src => src.ResponseMessage));
    }
}